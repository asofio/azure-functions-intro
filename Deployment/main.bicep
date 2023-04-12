param appName string = 'functions-intro-${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location

var cleanAppName = replace(replace(appName, '-', ''), '_', '')

module functionapp 'function-app.bicep' = {
  name: '${appName}-fn'
  params: {
    appName: '${appName}-fn'
    storageAccountName: '${cleanAppName}sa'
    location: location
    appInsightsLocation: location
  }
}

module servicebus 'service-bus.bicep' = {
  name: '${cleanAppName}sb'
  params: {
    serviceBusNamespaceName: '${cleanAppName}sb'
    serviceBusQueueName: 'orderresult'
    location: location
  }
}

module cosmosdb 'cosmosdb.bicep' = {
  name: '${appName}-cosmosdb'
  params: {
    cosmosDBAccountName: '${appName}-cosmosdb'
    location: location
  }
}
