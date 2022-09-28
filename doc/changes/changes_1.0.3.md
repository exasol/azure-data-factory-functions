# Azure Data Factory Functions 1.0.4, released 2022-01-04

Code name: Update for reported CVE in Azure.Storage.Blobs 

## Summary

In this release, we added and are now using the C# Exasol error code library for error handling. This makes for clearer, more-uniform errors and allows us to generate information needed for the Exasol error-catalog.

## Features

* #15: Add and use Exasol error code library for error handling.
* Github workflows - release on tag.

## Dependency Updates

* Added `error-reporting-csharp` version `0.1.0`