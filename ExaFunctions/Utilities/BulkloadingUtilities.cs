using System;
using System.Collections.Generic;
using System.Text;

namespace Exasol.Utilities
{
    public class BulkloadingUtilities
    {
        public static string ConstructBulkLoadingQuery(string dbTable, string azureStorageAccountConnectionString, string azureStorageAccountContainerName, List<string> fileNamesList)
        {
            var sbBLQ = new StringBuilder();

            string azureStorageAccountName = RegexUtilities.GrabValueFromAzureConnectionString(azureStorageAccountConnectionString, "AccountName");
            string azureStorageAccountKey = RegexUtilities.GrabValueFromAzureConnectionString(azureStorageAccountConnectionString, "AccountKey");
            //I don't know what the max length is but later on we can maybe split this up and return a list of queries to execute if needed
            //USER is the storage account name, IDENTIFIED BY wants a azure storage account key
            sbBLQ.Append($@"IMPORT INTO {dbTable} FROM CSV AT CLOUD AZURE BLOBSTORAGE 'DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net' USER '{azureStorageAccountName}' IDENTIFIED BY '{azureStorageAccountKey}' ");

            foreach (var filename in fileNamesList)
            {
                sbBLQ.Append($@"FILE '{azureStorageAccountContainerName}/{filename}' ");
            }
            return sbBLQ.ToString();
        }
    }
}