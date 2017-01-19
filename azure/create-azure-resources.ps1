<#
 .SYNOPSIS
    Creates all required infrastructure for the Azure Reference Web App

 .DESCRIPTION
    The following resources will be created:
    1) Basic App Service Plan
    2) Azure Web App 
    3) Standard_LRS Storage account

 .PARAMETER subscriptionId
    The subscription id where the resources will be deployed.
#>

param(
 [Parameter(Mandatory=$True)]
 [string]
 $subscriptionId
)

$ErrorActionPreference = "Stop"
$nameSuffix = (get-random -Count 6 -InputObject "123456".ToCharArray()) -join ''
$resourceLocation = "North Europe"
$resourceGroupName = "azure-reference-web"
$servicePlanName = "ref-app-plan"
$webAppName = "refwebapp$nameSuffix"
$storageName = "refwebappstg$nameSuffix"

# Sign in
Write-Host "Logging in...";
Login-AzureRmAccount;

# Select subscription
Write-Host "Selecting subscription '$subscriptionId'";
Select-AzureRmSubscription -SubscriptionID $subscriptionId;

# Create resources
Write-Host "Creating resources...";
New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceLocation
$plan = New-AzureRmAppServicePlan -ResourceGroupName $resourceGroupName -Name $servicePlanName -Location $resourceLocation -Tier "Basic" -NumberofWorkers 1 -WorkerSize "Small"
$webApp = New-AzureRmWebApp -Name $webAppName -ResourceGroupName $resourceGroupName -Location $resourceLocation -AppServicePlan "$servicePlanName"
$storage = New-AzureRmStorageAccount -ResourceGroupName $resourceGroupName -Name $storageName -Location $resourceLocation -SkuName Standard_LRS -Kind Storage
$storageKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName $storage.StorageAccountName)[0].Value
$storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=$storageName;AccountKey=$storageKey"

# Configure web app
Write-Host "Configuring web app...";

$appSettings = @{
    "ImageProcessQueueName" = "process"; 
    "ListingsTableName" = "listings"; 
    "ImagesContainerName" = "images";
    "IncomingContainerName" = "incoming";
};

$connectionStrings = @{
    "StorageConnectionString" = @{"Type" = "Custom"; "Value" = "$storageConnectionString"};
    "AzureWebJobsDashboard" = @{"Type" = "Custom"; "Value" = "$storageConnectionString"};
    "AzureWebJobsStorage" = @{"Type" = "Custom"; "Value" = "$storageConnectionString"};
};

$webApp | Set-AzureRmWebApp -ResourceGroupName $resourceGroupName -AppSettings $appSettings
$webApp | Set-AzureRmWebApp -ResourceGroupName $resourceGroupName -ConnectionStrings $connectionStrings

# Configure storage
Write-Host "Configuring storage...";
$storage | New-AzureStorageContainer -Name $appSettings["ImagesContainerName"] -Permission Blob
$storage | New-AzureStorageContainer -Name $appSettings["IncomingContainerName"] -Permission Off
$storage | New-AzureStorageQueue -Name $appSettings["ImageProcessQueueName"]
$storage | New-AzureStorageTable -Name $appSettings["ListingsTableName"]

Write-Host "Infrastructure created and configured!";