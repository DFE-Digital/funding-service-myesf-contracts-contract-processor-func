# Manage your education and skills funding - Contracts (change events) processor

The Manage Your Education and Skills Funding (MYESF) Contracts Events Processor is used by the MYESF web application to allow the following:

- Retrieval of changes of contract status from the Funding Contract Service (FCS)
- Sending such data to the Contracts Data API
- Retrieval of documents from a Sharepoint library

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 3.1 function app utilising Azure App Service for deployment.

The function app runs on an Azure App service on Azure.

It is a serverless azure function that processes contract change events based on an atom feed, that is produced by the [feed processor](https://github.com/DFE-Digital/funding-service-myesf-contracts-feed-processor-func). It is triggerd by a service bus message, with sessions enabled for orderly processing of contract events.

As part of the contract creation process, it also requests contract documents from the Sharepoint client service that accesses a Sharepoint library, then calls the contract data API to request a contract to be created.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `local.settings.json` file will need to be created in the Pds.Contracts.ContractEventProcessor.Func project. Below, and included in the repo, there is an `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "IsEncrypted": false,
  "Version": "2.0",
  "Values": {
    "ServiceBusConnection": "",
    "Environment": "local",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "FUNCTIONS_EXTENSION_VERSION": "~3",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "",
    "PdsApplicationInsights:InstrumentationKey": "",
    "PdsApplicationInsights:Environment": "local",
    "ContractEventsSessionQueue": "",
    "ContractsDataApiConfiguration:ApiBaseAddress": "",
    "ContractsDataApiConfiguration:Authority": "https://login.microsoftonline.com/",
    "ContractsDataApiConfiguration:TenantId": "",
    "ContractsDataApiConfiguration:ClientId": "",
    "ContractsDataApiConfiguration:ClientSecret": "",
    "ContractsDataApiConfiguration:AppUri": "",
    "ContractsDataApiConfiguration:ShouldSkipAuthentication": false,
    "AuditApiConfiguration:ApiBaseAddress": "",
    "AuditApiConfiguration:Authority": "https://login.microsoftonline.com/",
    "AuditApiConfiguration:TenantId": "",
    "AuditApiConfiguration:ClientId": "",
    "AuditApiConfiguration:ClientSecret": "",
    "AuditApiConfiguration:AppUri": "",
    "MaximumDeliveryCount": 9,
    "SPClientServiceConfiguration:ApiBaseAddress": "",
    "SPClientServiceConfiguration:Authority": "https://accounts.accesscontrol.windows.net/",
    "SPClientServiceConfiguration:ClientId": "",
    "SPClientServiceConfiguration:ClientSecret": "",
    "SPClientServiceConfiguration:TenantId": "",
    "SPClientServiceConfiguration:AppUri": "",
    "SPClientServiceConfiguration:Resource": "00000003-0000-0ff1-ce00-000000000000",
    "SPClientServiceConfiguration:RelativeSiteURL": "",
    "SPClientServiceConfiguration:PublicationFolderSuffix": "",
    "SPClientServiceConfiguration:ShouldErrorPdfNotFound": true,
    "SPClientServiceConfiguration:AADClientId": "",
    "SPClientServiceConfiguration:AADClientSecret": "",
    "FeatureFlag:UseSPAzureADAuthentication": false,
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE": true,
    "WEBSITE_RUN_FROM_PACKAGE": 1
  }
}
```

### Setting Details

- **`AzureWebJobsStorage`**
  The core application setting used by the Azure Functions and Azure WebJobs runtime to establish a connection to an Azure Storage account.

- **`FUNCTIONS_EXTENSION_VERSION`**      
  The functions extension version number.

- **`FUNCTIONS_WORKER_RUNTIME`**
  The functions runtime mode.

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for Application Insights resource for logging purposes.
   
- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.
 
- **`ContractsDataApiConfiguration:ApiBaseAddress`** 
  The base URL endpoint used by a client application to route network requests to the Contracts Data API backend.

- **`ContractsDataApiConfiguration:Authority`** 
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the Contracts Data API client.

- **`ContractsDataApiConfiguration:TenantId`** 
  The unique identifier for your azure ad tenant.

- **`ContractsDataApiConfiguration:ClientId`** 
  The Contracts Data API application (client) ID registered in azure ad.

- **`ContractsDataApiConfiguration:ClientSecret`** 
  The confidential credential used by the Contracts Data API application to securely prove its identity to the Identity Provider.

- **`ContractsDataApiConfiguration:AppUri`** 
  The unique Application ID URI used as the identifier for the protected Contracts Data API resource within the Identity Provider.

- **`AuditApiConfiguration:ApiBaseAddress`** 
  The base URL endpoint used by a client application to route network requests to the Audit API backend.

- **`AuditApiConfiguration:Authority`** 
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the Audit API client.

- **`AuditApiConfiguration:TenantId`** 
  The unique identifier for your azure ad tenant.

- **`AuditApiConfiguration:ClientId`** 
  The Audit API application (client) ID registered in azure ad.

- **`AuditApiConfiguration:ClientSecret`** 
  The confidential credential used by the Audit API application to securely prove its identity to the Identity Provider.

- **`AuditApiConfiguration:AppUri`** 
  The unique Application ID URI used as the identifier for the protected Audit API resource within the Identity Provider.

- **`SPClientServiceConfiguration:ApiBaseAddress`**
  The base URL endpoint used by a client application to route network requests to the Sharepoint Client Service.

- **`SPClientServiceConfiguration:Authority`**
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the Sharepoint Client Service.

- **`SPClientServiceConfiguration:ClientId`** 
  The Sharepoint Client Service (client) ID registered in azure ad.

- **`SPClientServiceConfiguration:ClientSecret`** 
  The confidential credential used by the Sharepoint Client Service to securely prove its identity to the Identity Provider.

- **`SPClientServiceConfiguration:TenantId`** 
  The unique identifier for your azure ad tenant.

- **`SPClientServiceConfiguration:AppUri`** 
  The unique Application ID URI used as the identifier for the protected the Sharepoint Client Service resource within the Identity Provider.

- **`SPClientServiceConfiguration:RelativeSiteURL`** 
  The relative URL for the Sharepoint site.

- **`SPClientServiceConfiguration:PublicationFolderSuffix`** 
  The suffix at the end of the name of each document library folder in the Sharepoint library.

- **`SPClientServiceConfiguration:ShouldErrorPdfNotFound`** 
  Determines if an error should be thrown if a Pdf is not found in the Sharepoint library.

- **`SPClientServiceConfiguration:AADClientId`** 
  The SharePoint Graph Client Service (client) ID registered in azure ad.

- **`SPClientServiceConfiguration:AADClientSecret`** 
  The confidential credential used by the SharePoint Graph Client Service to securely prove its identity to the Identity Provider.

- **`FeatureFlag:UseSPAzureADAuthentication`** 
  Determines whether to use SharePoint Graph Client Service.
 
## Build and Test

This API is built using

* Microsoft Visual Studio 2019
* .Net Core 3.1

To build and test locally, you can either use Visual Studio 2019 or Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

* If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
* If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
