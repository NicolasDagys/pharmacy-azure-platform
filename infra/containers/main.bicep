targetScope = 'resourceGroup'

param environment string
param apiImage string
param frontendImage string
param keyVaultUri string
param managedIdentityResourceId string
param appInsightsConnectionString string
param environmentId string
param acrLoginServer string
param openAiEndpoint string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: 'pharmacy-mi-${environment}'
}

module acrPull './modules/acrPullRoleAssignment.bicep' = {
  name: 'acrPull'
  params: {
    acrName: 'pharmacyacr${environment}'
    principalId: managedIdentity.properties.principalId
  }
}

module api './modules/apiContainerApp.bicep' = {
  name: 'api'
  dependsOn: [
    acrPull
  ]
  params: {
    appName: 'pharmacy-api-${environment}'
    location: resourceGroup().location
    environmentId: environmentId
    managedIdentityResourceId: managedIdentityResourceId
    keyVaultUri: keyVaultUri
    acrLoginServer: acrLoginServer
    apiImage: apiImage
    appInsightsConnectionString: appInsightsConnectionString
    openAiEndpoint: openAiEndpoint
  }
}

module frontend './modules/frontendContainerApp.bicep' = {
  name: 'frontend'
  dependsOn: [
    acrPull
  ]
  params: {
    appName: 'pharmacy-web-${environment}'
    location: resourceGroup().location
    environmentId: environmentId
    frontendImage: frontendImage
    acrLoginServer: acrLoginServer
    managedIdentityResourceId: managedIdentityResourceId
    apiUrl: api.outputs.apiUrl
  }
}

output apiUrl string = api.outputs.apiUrl
output frontendUrl string = frontend.outputs.frontendUrl
