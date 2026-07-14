# Manage your education and skills funding - Contracts (change events) processor

## Introduction

Contracts processor is a serverless azure function that process contract change events based on atom feed, that is produced by the [feed processor](https://github.com/SkillsFundingAgency/pds-contracts-func-feed-processor). This function is a service bus triggered function, with session enabled for orderly processing of contract events.

### Getting Started

This product is a Visual Studio 2019 solution containing several projects (Azure function application and service layers, with associated unit test and integration test projects).
To run this product locally, you will need to configure the list of dependencies, once configured and the configuration files updated, it should be F5 to run and debug locally.

### Installing

Clone the project and open the solution in Visual Studio 2019.

#### List of dependencies

|Item |Purpose|
|-------|-------|
|Azure Storage Emulator| The Microsoft Azure Storage Emulator is a tool that emulates the Azure Blob, Queue, and Table services for local development purposes. This is required for webjob storage used by azure functions.|
|Azure function development tools | To run and test azure functions locally. |
|Azure service bus | When the feeds are processed, a message will be created for the contract processor to continue processing of contract events. Service bus cannot be set up locally, you will need an azure subscription to set-up. |
|Contracts API | API for managing contracts. |
|SharePoint Online document library | Where the contract document can be found (internal only). |

#### Azure Storage Emulator

The Storage Emulator is available as part of the Microsoft Azure SDK. Azure functions require it for local development.

#### Azure function development tools

You can use your favourite code editor and development tools to create and test functions on your local computer.
We used visual studio and Azure core tools CLI for development and testing. You can find more information for your favourite code editor at <https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-local>.

* Using Visual Studio - To develop functions using visual studio, include the Azure development workload in your Visual Studio installation. More detailed information can be found at <https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs>.
* Azure Functions Core Tools - These tools provide CLI with core runtime and templates for creating functions, which can be used to develop and run functions without visual studio. This can be installed using package managers like `npm` or `chocolately` more detailed information can be found at <https://www.npmjs.com/package/azure-functions-core-tools>.

#### Azure service bus

Microsoft Azure Service Bus is a fully managed enterprise message broker.
Publish-subscribe topics are used by this application to decouple approval processing.
There are no emulators available for azure service bus, hence you will need an azure subscription and set-up a service bus namesapce with a topic created to run this application.
Once you have set-up an azure service bus namespace, you will need to create a shared access policy to set in local configuration settings.

#### Contracts API

Contract API can be found at <https://github.com/SkillsFundingAgency/pds-contracts-data-api>.

#### SharePoint Online document library

You can configure any SharePoint online document library, and register an app to create an oauth client credentials. This can then be added to the config to access documents from library.

### Local Config Files

Once you have cloned the public repo you need the following configuration files listed below.

| Location | config file |
|-------|-------|
| Pds.Contracts.ContractEventProcessor.Func | local.settings.json |

The following is a sample configuration file

```json
{
  "IsEncrypted": false,
  "version": "2.0",
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ContractEventsSessionQueue": "replace_QueueName",
    "ServiceBusConnection": "replace_ServiceBusConnectionString",

    "MaximumDeliveryCount": 10,

    "SPClientServiceConfiguration:ApiBaseAddress": "replace_sharepoint_online_baseaddress",
    "SPClientServiceConfiguration:Authority": "https://accounts.accesscontrol.windows.net/",
    "SPClientServiceConfiguration:ClientId": "replace_clientid_guid",
    "SPClientServiceConfiguration:ClientSecret": "replace_clientsecret",
    "SPClientServiceConfiguration:TenantId": "replace_tenantid",
    "SPClientServiceConfiguration:AppUri": "replace_sharepoint_host",
    "SPClientServiceConfiguration:Resource": "replace_app_resouce_id",
    "SPClientServiceConfiguration:RelativeSiteURL": "replace_relative_siteurl",
    "SPClientServiceConfiguration:PublicationFolderSuffix": "replace_document_library_name",
    "SPClientServiceConfiguration:ShouldErrorPdfNotFound": true_or_false
  },

  "ContractsDataApiConfiguration": {
    "ApiBaseAddress": "replace_local_contract_api_or_stub",
    "ShouldSkipAuthentication": "true"
    },
}
```

The following configurations need to be replaced with your values.
|Key|Token|Example|
|-|-|-|
|SPClientServiceConfiguration.ApiBaseAddress|replace_sharepoint_online_baseaddress|<http://localhost:5001>|
|SPClientServiceConfiguration.ClientId|replace_clientid_guid|A valid guid.|
|SPClientServiceConfiguration.ClientSecret|replace_clientsecret|Any string.|
|SPClientServiceConfiguration.TenantId|replace_tenantid|SharePoint online tenant id.|
|SPClientServiceConfiguration.AppUri|replace_sharepoint_host|A SharePoint hostname e.g. fabrikam.sharepoint.com|
|SPClientServiceConfiguration.Resource|replace_app_resouce_id|SharePoint Online application principal ID.|
|SPClientServiceConfiguration.RelativeSiteURL|replace_relative_siteurl|/sites/contoso|
|SPClientServiceConfiguration.PublicationFolderSuffix|replace_document_library_name|This is suffix of document library, see `GetFolderNameForContractDocument` method of [ContractEventMapper.cs](Pds.Contracts.ContractEventProcessor\Pds.Contracts.ContractEventProcessor.Services\Implementations\ContractEventMapper.cs) for document library format. |
|SPClientServiceConfiguration.ShouldErrorPdfNotFound|true_or_false|`false` to continue with a dummy contract document, you still need to ensure a valid SharePoint access token can be created.|
|AuditApiConfiguration.ApiBaseAddress|replace_local_audit_api_or_stub|<http://localhost:5002/>|
|ServiceBusConnectionString|replace_ServiceBusConnectionString|A valid azure service bus connection string|
|ContractEventsSessionQueue|replace_QueueName|atom-feed-queue|
|ContractsDataApiConfiguration.ApiBaseAddress|replace_local_contract_api_or_stub|<http://localhost:5002>|

## Build and Test

This API is built using

* Microsoft Visual Studio 2019
* .Net Core 3.1

To build and test locally, you can either use visual studio 2019 or VSCode or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

* If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
* If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
