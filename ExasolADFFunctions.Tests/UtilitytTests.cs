using Exasol.Utilities;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExasolADFFunctions.Tests
{
    public class UtilitytTests
    {
        [Fact]
        public void BulkloadingQueryFnOutput()
        {
            List<String> testFileNameList = new List<string>();
            testFileNameList.Add("Filename1.csv");
            testFileNameList.Add("Filename2.csv");
            testFileNameList.Add("Filename3.csv");
            var output = BulkloadingUtilities.ConstructBulkLoadingQuery("dbTable", "azureStorageAccountconnectionString;AccountKey=accKey;AccountName=accName;", "azureStorageAccountContainerName", testFileNameList);

            var expectedOutput = "IMPORT INTO dbTable FROM CSV AT CLOUD AZURE BLOBSTORAGE 'DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net' USER 'accName' IDENTIFIED BY 'accKey' FILE 'azureStorageAccountContainerName/Filename1.csv' FILE 'azureStorageAccountContainerName/Filename2.csv' FILE 'azureStorageAccountContainerName/Filename3.csv' ";
            Assert.True(output == expectedOutput);
                }
        [Fact]
        public void ConnStringKeyRegexExtractionTest()
        {
            var key =RegexUtilities.GrabValueFromAzureConnectionString("test;AccountKey=akey1;","AccountKey");
            Assert.True(key == "akey1");

        }
        [Fact]
        public void FilterFileNameDirPath()
        {
            List<string> filenames = new List<string>();
            filenames.Add("fileA");
            filenames.Add("fileB");
            filenames.Add("pathA/pathB/fileRes");
            var res = FilterUtilities.FilterFilenameListOnDirectoryPath(filenames,"pathA/pathB/");
            Assert.True(res.Count == 1);
        }
        [Fact]
        public void FilterFileNameNoDirPath()
        {
            List<string> filenames = new List<string>();
            filenames.Add("fileA");
            filenames.Add("fileB");
            filenames.Add("pathA/pathB/fileRes");
            var res = FilterUtilities.FilterFilenameListOnDirectoryPath(filenames, null);
            Assert.True(res.Count == 3);
        }
    }
}
