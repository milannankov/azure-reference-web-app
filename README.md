![App Architecture](resources/reference-app-architecture.png)
# Azure Reference Web App

Azure reference Web App is a web application that showcases how different Azure services and features can be utilized to create a robus and scalable web app.
The App utilizes the following Azure services and features: `Azure Web Apps`, `Azure WebJobs`, `Azure Storage` (`Table`, `Queue`, and `Blob`). 

For more information about how each Azure service is used refer to the following blog post - [Azure Reference Web App](https://www.newventuresoftware.com/blog/azure-reference-web-app)

# Running The Azure Reference Web App

The code of the app is available on Github but before you can run the app you need to create the required Azure infrastructure. Fortunately there is a PowerShell script that will create everything for you. Once you clone the Git repository, navigate to the "azure" folder and execute create-azure-resources.ps1. You will have to enter your Azure subscription id where the resources will be deployed.

Once the infrastructure is in place, you can deploy the app to Azure [using Visual Studio](https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-dotnet-get-started) or any of the other methods available. 

In case you do not have an Azure account you can create a [Free trial account](https://azure.microsoft.com/en-us/free/). 