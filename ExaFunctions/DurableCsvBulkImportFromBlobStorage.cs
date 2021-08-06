using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Exasol.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Exasol
{
    //http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-4-Passing-Input-To-Orchestrations-and-Activities
    //https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-create-first-csharp?pivots=code-editor-visualstudio
    //https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-error-handling?tabs=csharp
    public static class DurableCsvBulkImportFromBlobStorage
    {
        [FunctionName("DurableCsvBulkImportFromBlobStorage")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {

            ImportOrchestratorParams iop = context.GetInput<ImportOrchestratorParams>();
            //get the filenames
            var filenamesList = await context.CallActivityAsync<List<string>>("DurableCsvBulkImportFromBlobStorage_GetFileNames", iop);
            //divide them in batches 
            var filenameBatches = filenamesList.Batch(iop.FilesProcessedInParallel.Value).ToList();
            //https://stackoverflow.com/questions/11463734/split-a-list-into-smaller-lists-of-n-size
            var outputs = new List<string>();
            int batchNr = 1;
            foreach (var filenamesBatch in filenameBatches)
            {
                var bulkImportParams = new BulkImportParams { IoParams = iop, FileNames = filenamesBatch.ToList(), BatchNr = batchNr };
                var retValue = await context.CallActivityAsync<string>("DurableCsvBulkImportFromBlobStorage_RunBulkImporter", bulkImportParams);
                outputs.Add(retValue);
                batchNr++;
            }

            return outputs;
        }
        public class BulkImportParams
        {
            public int BatchNr { get; set; }
            public List<string> FileNames { get; set; }
            public ImportOrchestratorParams IoParams { get; set; }
        }

        [FunctionName("DurableCsvBulkImportFromBlobStorage_GetFileNames")]
        public static List<string> GetFileNames([ActivityTrigger] ImportOrchestratorParams iop, ILogger log)
        {
            log.LogInformation($"Fetching filenames list.");
            List<string> filenameList;
            try
            {
                filenameList = AzureUtilities.GetAzureFilesListInBlobContainerFolder(iop.AzureStorageAccountConnectionString, iop.AzureStorageAccountContainerName);
                return FilterUtilities.FilterFilenameListOnDirectoryPath(filenameList, iop.AzureStorageAccountContainerPath);
            } catch (Exception e)
            {
                throw new Exception(e.Message); //this looks silly but there were issues with (de)serializing the more complex exceptions and this at least gets relevant information to the end user.
            }

        }

        [FunctionName("DurableCsvBulkImportFromBlobStorage_RunBulkImporter")]
        public static string RunBulkImporter([ActivityTrigger] BulkImportParams bio, ILogger log)
        {
            
            log.LogInformation($"Running bulk import for batch {bio.BatchNr}");
            try
            {
                string bulkloadingQuery = BulkloadingUtilities.ConstructBulkLoadingQuery(bio.IoParams.DbTable, bio.IoParams.AzureStorageAccountConnectionString, bio.IoParams.AzureStorageAccountContainerName, bio.FileNames);
                var response = QueryUtilities.RunNonQuery(bio.IoParams.DbConnectionString, bulkloadingQuery);
            } catch (Exception e)
            {
                throw new Exception(e.Message); //this looks silly but there were issues with (de)serializing the more complex exceptions and this at least gets relevant information to the end user.
            }
            return $"Batch {bio.BatchNr}: OK";
        }

        public class ImportOrchestratorParams
        {
            public string DbConnectionString { get; set; }
            //the target db table
            public string DbTable { get; set; }
            //the azure storage account connection string
            public string AzureStorageAccountConnectionString { get; set; }
            //the name of the blob storage container withing the storage account
            public string AzureStorageAccountContainerName { get; set; }
            //the filepath within the blob storage container
            public string AzureStorageAccountContainerPath { get; set; }
            //the batch size of the files to process
            public int? FilesProcessedInParallel { get; set; }
        }
        [FunctionName("DurableCsvBulkImportFromBlobStorage_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {

                var content = req.Content;
                string jsonContent = await content.ReadAsStringAsync();
                dynamic data = RequestUtilities.DeserializeJsonBody(jsonContent);

                CheckParameters(data);

                var importParams = new ImportOrchestratorParams();
                ////the target db connection string
                importParams.DbConnectionString = data.dbconnectionstring;
                //the target db table
                importParams.DbTable = data.dbtable;
                //the azure storage account connection string
                importParams.AzureStorageAccountConnectionString = data.storageaccountconnectionstring;
                //the name of the blob storage container withing the storage account
                importParams.AzureStorageAccountContainerName = data.storageaccountcontainername;
                //the filepath within the blob storage container
                //TODO: make this a wildcard/optional
                importParams.AzureStorageAccountContainerPath = data.storageaccountcontainerpath;
                //the batch size of the files to process
                importParams.FilesProcessedInParallel = data.filesprocessedinparallel;

                // Function input comes from the request content.
                string instanceId = await starter.StartNewAsync("DurableCsvBulkImportFromBlobStorage", importParams);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                response.ReasonPhrase = e.Message;
                return response;
            }

        }

        private static void CheckParameters(dynamic data)
        {

            ////the target db connection string
            if (data.dbconnectionstring == null)
            {
                throw new Exception("The database connection string was not included in the request and is required. Use \"dbconnectionstring\" to add this value to the request.");
            }
            //the target db table
            if (data.dbtable == null)
            {
                throw new Exception("The database table name was not included in the request and is required. Use \"dbtable\" to add this value to the request.");
            }
            //the azure storage account connection string
            if (data.storageaccountconnectionstring == null)
            {
                throw new Exception("The storage account connection string was not included in the request and is required. Use \"storageaccountconnectionstring\" to add this value to the request.");
            }
            //the name of the blob storage container withing the storage account
            if (data.storageaccountcontainername == null)
            {
                throw new Exception("The storage account container name was not included in the request and is required. Use \"storageaccountcontainername\" to add this value to the request.");
            }
            //the filepath within the blob storage container
            //OPTIONAL : make this a wildcard/optional
            //if (data.storageaccountcontainerpath)
            //           {

            //           }
            //OPTIONAL the nr of files to process in parallel
            if (data.filesprocessedinparallel == null)
            {
                data.filesprocessedinparallel = 1;
            }

        }
    }
}