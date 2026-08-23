targetScope = 'subscription'

param location string
param resourceGroupName string
param environment string

param sqlAdminUser string

@secure()
param administratorPassword string

@secure()
param jwtKey string

param jwtIssuer string
param jwtAudience string

param actionGroupEmail string

param currentPublicIp string

resource pharmacyResourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
}

// ------------------------------------------------------------
// Database
// ------------------------------------------------------------

module data './data/main.bicep' = {
  name: 'data'

  scope: pharmacyResourceGroup

  dependsOn: [
    pharmacyResourceGroup
  ]

  params: {
    environment: environment
    sqlAdminUser: sqlAdminUser
    administratorPassword: administratorPassword
    currentPublicIp: currentPublicIp
  }
}

// ------------------------------------------------------------
// Azure OpenAI
// ------------------------------------------------------------

module openAi './ai/modules/openai.bicep' = {
  name: 'openAi'

  scope: pharmacyResourceGroup

  dependsOn: [
    pharmacyResourceGroup
  ]

  params: {
    openAiName: 'pharmacy-ai-${environment}'
    location: location
  }
}

// ------------------------------------------------------------
// Security
// ------------------------------------------------------------

module security './security/main.bicep' = {
  name: 'security'

  scope: pharmacyResourceGroup

  dependsOn: [
    data
    openAi
  ]

  params: {
    environment: environment

    sqlAdminUser: sqlAdminUser
    sqlAdminPassword: administratorPassword

    jwtKey: jwtKey
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience

    sqlServerName: data.outputs.sqlServerName
    databaseName: data.outputs.databaseName

    secrets: [
      {
        name: 'openai-api-key'
        value: openAi.outputs.openAiKey
      }
    ]
  }
}

// ------------------------------------------------------------
// Monitoring
// ------------------------------------------------------------

module monitoring './monitoring/main.bicep' = {
  name: 'monitoring'

  scope: pharmacyResourceGroup

  dependsOn: [
    security
  ]

  params: {
    environment: environment
    actionGroupEmail: actionGroupEmail
  }
}

// ------------------------------------------------------------
// ACR
// ------------------------------------------------------------

module acr './containers/modules/acr.bicep' = {
  name: 'acr'

  scope: pharmacyResourceGroup

  dependsOn: [
    pharmacyResourceGroup
  ]

  params: {
    acrName: 'pharmacyacr${environment}'
    location: location
  }
}

// ------------------------------------------------------------
// Container Apps Environment
// ------------------------------------------------------------

module containerAppEnvironment './containers/modules/containerAppEnvironment.bicep' = {
  name: 'containerAppEnvironment'

  scope: pharmacyResourceGroup

  dependsOn: [
    monitoring
    security
    openAi
  ]

  params: {
    environmentName: 'pharmacy-cae-${environment}'
    location: location
    logAnalyticsWorkspaceId: monitoring.outputs.workspaceId
  }
}

// ------------------------------------------------------------
// Outputs
// ------------------------------------------------------------

output resourceGroupName string = pharmacyResourceGroup.name

output sqlServerName string = data.outputs.sqlServerName

output databaseName string = data.outputs.databaseName

output appInsightsId string = monitoring.outputs.appInsightsId

output actionGroupId string = monitoring.outputs.actionGroupId

output keyVaultUri string = security.outputs.keyVaultUri

output managedIdentityResourceId string = security.outputs.managedIdentityId

output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString

output containerAppEnvironmentId string = containerAppEnvironment.outputs.environmentId

output acrLoginServer string = acr.outputs.acrLoginServer

output openAiEndpoint string = openAi.outputs.openAiEndpoint
