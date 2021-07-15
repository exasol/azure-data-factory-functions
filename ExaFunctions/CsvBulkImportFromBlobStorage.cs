using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Exasol.Utilities;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Exasol
{
    public static class CsvBulkImportFromBlobStorage
    {
        [FunctionName("CsvBulkImportFromBlobStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = RequestUtilities.DeserializeJsonBody(requestBody);

                CheckParameters(data);
                //POST body
                //the target db connection string
                string dbConnectionString = data?.dbconnectionstring;
                //the target db table
                string dbTable = data?.dbtable;
                //the azure storage account connection string
                string azureStorageAccountConnectionString = data?.storageaccountconnectionstring;
                //the name of the blob storage container withing the storage account
                string azureStorageAccountContainerName = data?.storageaccountcontainername;
                //the filepath within the blob storage container
                string azureStorageAccountContainerPath = data?.storageaccountcontainerpath;

                var filenameList = AzureUtilities.GetAzureFilesListInBlobContainerFolder(azureStorageAccountConnectionString, azureStorageAccountContainerName);
                var filteredFilenameList = FilterUtilities.FilterFilenameListOnDirectoryPath(filenameList, azureStorageAccountContainerPath);

                string bulkloadingQuery = BulkloadingUtilities.ConstructBulkLoadingQuery(dbTable, azureStorageAccountConnectionString, azureStorageAccountContainerName, filteredFilenameList);
                var response = QueryUtilities.RunNonQuery(dbConnectionString, bulkloadingQuery);
                return new OkObjectResult(response);

            }
            catch (Exception e)
            {
                var errorObject = new JObject();
                errorObject.Add("Something went wrong:", e.Message);

                // Return a 400 bad request result to the client with JSON body
                return new BadRequestObjectResult(errorObject);
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

        }


    }
}
