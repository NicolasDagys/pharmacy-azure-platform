param sqlServerName string
param databaseName string
param location string

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  name: '${sqlServerName}/${databaseName}'

  sku: {
    name: 'Basic'
    tier: 'Basic'
  }

  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
  location:location
}

output databaseId string = database.id
output databaseName string = database.name
