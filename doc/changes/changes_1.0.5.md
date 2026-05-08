# Azure Data Factory Functions 1.0.5, released 2026-05-08

Code name: Update dependencies, target newer .NET version

## Summary

In this release, we updated dependencies and target .NET version 10.0.0 (Core 3.1 is end-of-life).

## Dependency Updates

### `ExaFunctions/ExasolADFFunctions.csproj`

* Updated `TargetFramework` from `netcoreapp3.1` to `net10.0`
* Updated `Azure.Storage.Blobs` from `12.13.1` to `12.27.0`
* Updated `Microsoft.Azure.WebJobs.Extensions.DurableTask` from `2.1.1` to `3.12.4`
* Updated `Microsoft.Extensions.Configuration.UserSecrets` from `3.1.13` to `10.0.7`
* Updated `Microsoft.NET.Sdk.Functions` from `3.0.11` to `4.6.0`
* Updated `morelinq` from `3.3.2` to `4.4.0`

### `ExasolADFFunctions.Tests/ExasolADFFunctions.Tests.csproj`

* Updated `TargetFramework` from `netcoreapp3.1` to `net10.0`
* Updated `Microsoft.NET.Test.Sdk` from `16.9.4` to `18.5.1`
* Updated `xunit.runner.visualstudio` from `2.4.3` to `3.1.5`
* Updated `coverlet.collector` from `3.0.2` to `10.0.0`
* Replaced `xunit` version `2.4.1` with `xunit.v3` version `3.2.2`
