Usage:

Copy/Rename/Delete the example functions as necessary for the requirements of your project.
For running locally, if you are using anything other than HTTP triggered functions, add an app settings file called "local.settings.json" with content as follows:

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "ServiceBusConnection": "[your Azure ServiceBus connection string]"
  }
}

Note that the "ServiceBusConnection" setting is only required if you are using a ServiceBus message triggered function.