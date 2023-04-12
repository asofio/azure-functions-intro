param cosmosDBAccountName string
param location string = resourceGroup().location
param dbName string = 'tax'

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: cosmosDBAccountName
  location: location
  properties:{
    databaseAccountOfferType:'Standard'
    enableAutomaticFailover:false
    enableMultipleWriteLocations:false
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  parent: cosmosDbAccount
  name: dbName
  properties:{
    resource:{
      id: dbName
    }
  }
}

resource orderContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  parent: cosmosDb
  name:'salesTax'
  properties:{
    resource:{
      id: 'salesTax'
      partitionKey:{
        paths:[
          '/id'
        ]
      }
    }
  }
}
