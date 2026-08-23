targetScope = 'resourceGroup'
param environment string
param sqlAdminUser string
@secure()
param sqlAdminPassword string
param jwtKey string
param jwtIssuer string
param jwtAudience string
param sqlServerName string
param databaseName string
param secrets array

module managedIdentity './modules/managedIdentity.bicep' = {
  name: 'managedIdentity'

  params: {
    identityName: 'pharmacy-mi-${environment}'
    location: resourceGroup().location
  }
}

module keyVault './modules/keyVault.bicep' = {
  name: 'keyVault'

  dependsOn: [
    managedIdentity
  ]

  params: {
    keyVaultName: 'pharmacy-kv-${environment}'
    location: resourceGroup().location

    sqlAdminUser: sqlAdminUser
    administratorPassword: sqlAdminPassword

    jwtKey: jwtKey
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience

    sqlConnectionString: 'Server=tcp:${sqlServerName}.database.windows.net,1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlAdminUser};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

    secrets: secrets
  }
}
module keyVaultRoleAssignment './modules/keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultRoleAssignment'
  dependsOn: [
    keyVault
    managedIdentity
  ]
  params: {
    keyVaultName: 'pharmacy-kv-${environment}'
    principalId: managedIdentity.outputs.principalId
  }
}

output keyVaultId string = keyVault.outputs.keyVaultId
output keyVaultUri string = keyVault.outputs.keyVaultUri
output keyVaultName string = keyVault.outputs.keyVaultName
output managedIdentityId string = managedIdentity.outputs.identityId
output managedIdentityPrincipalId string = managedIdentity.outputs.principalId
