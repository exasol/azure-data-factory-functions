# Exasol Azure Data Factory functions

This repository contains auxiliary azure functions for use in Azure Data Factory pipelines.

Included so far are a Azure Blob Storage CSV bulk import function and a query function.

Please see the [user guide](doc/user_guide.md) for detailed instructions.

![](C:\repos\azure-data-factory-functions\img\2021-08-06-09-24-16-image.png)

The project uses [ADO.NET Data Provider | Exasol Documentation](https://docs.exasol.com/connect_exasol/drivers/ado_net.htm) for connecting and executing queries on the Exasol database.

The CSV bulk import function being utilized is documented here:

[Load Data from Azure&#160;Blob Storage | Exasol Documentation](https://docs.exasol.com/loading_data/load_data_azure_blob.htm)
