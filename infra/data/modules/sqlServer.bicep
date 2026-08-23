param sqlServerName string
param location string
@secure()
param administratorLogin string
@secure()
param administratorPassword string
param currentPublicIp string

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location

  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    version: '12.0'
    publicNetworkAccess: 'Enabled'
  }
}

resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  name: 'AllowAzureServices'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

//in order to run the inicialization script. we need a firewall rule to allow the current public IP address to access the SQL server. This is a temporary rule that will be removed after the script is executed.
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  name: 'AllowMyIP'
  parent: sqlServer

  properties: {
    startIpAddress: currentPublicIp
    endIpAddress: currentPublicIp
  }
}

output sqlServerId string = sqlServer.id
output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
