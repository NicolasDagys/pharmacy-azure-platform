targetScope = 'resourceGroup'
param environment string
param sqlAdminUser string
@secure()
param administratorPassword string
param currentPublicIp string

var sqlServerName = 'pharmacy-sql-${environment}-${uniqueString(resourceGroup().id)}'
var databaseName = 'Pharmacy'

module sqlServer './modules/sqlServer.bicep' = {
  name: 'sqlServer'
  params: {
    sqlServerName: sqlServerName
    location: resourceGroup().location
    administratorLogin: sqlAdminUser
    administratorPassword: administratorPassword
    currentPublicIp:currentPublicIp
  }
}

module sqlDatabase './modules/sqlDatabase.bicep' = {
  name: 'sqlDatabase'
  dependsOn: [
    sqlServer
  ]
  params: {
    sqlServerName: sqlServerName
    databaseName: databaseName
    location: resourceGroup().location
  }
}

output sqlServerName string = sqlServerName
output sqlServerFqdn string = sqlServer.outputs.sqlServerFqdn
output databaseName string = databaseName
