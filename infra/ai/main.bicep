targetScope = 'resourceGroup'
@description('Deployment environment, for example dev, test or prod.')
param environment string
@description('Key Vault URI where AI secrets will be stored.')
param keyVaultUri string

// ------------------------------------------------------------
// Azure OpenAI
// ------------------------------------------------------------

module openai './modules/openai.bicep' = {
  name: 'openai'
  params: {
    openAiName: 'pharmacy-openai-${environment}'
    location: resourceGroup().location
  }
}

// ------------------------------------------------------------
// Existing Key Vault
// ------------------------------------------------------------

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: last(split(keyVaultUri, '/'))
}

// ------------------------------------------------------------
// Azure OpenAI secrets
// ------------------------------------------------------------

resource openAiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-02' = {
  name: 'openai-api-key'
  parent: keyVault
  properties: {
    value: openai.outputs.openAiKey
  }
}

resource openAiEndpointSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-02' = {
  name: 'openai-endpoint'
  parent: keyVault
  properties: {
    value: openai.outputs.openAiEndpoint
  }
}
// ------------------------------------------------------------
// Outputs
// ------------------------------------------------------------
output openAiEndpoint string = openai.outputs.openAiEndpoint
output openAiKeyVaultSecretUri string = openAiKeySecret.properties.secretUri
output openAiEndpointKeyVaultSecretUri string = openAiEndpointSecret.properties.secretUri
