using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exasol
{
    class AzureUtilities
    {
        public static List<String> GetAzureFilesListInBlobContainerFolder(string storageAccountConnectionString, string containerName)
        {
            List<string> fileNameList = new List<string>();
            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(storageAccountConnectionString, containerName);

            // Print out all the blob names
            foreach (BlobItem blob in container.GetBlobs())
            {
                Console.WriteLine(blob.Name);
                fileNameList.Add(blob.Name);
            }
            return fileNameList;
        }
    }
}
