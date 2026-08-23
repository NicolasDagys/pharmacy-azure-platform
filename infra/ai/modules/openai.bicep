param openAiName string
param location string = resourceGroup().location
param skuName string = 'S0'

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: openAiName
  location: location
  kind: 'OpenAI'
  sku: {
    name: skuName
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    customSubDomainName: openAiName
  }
}

resource openAiDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  parent: openAi
  name: 'gpt-5-mini-deployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-5-mini'
      version: '2025-08-07'
    }
  }
  sku: {    
    name: 'GlobalStandard'
    capacity: 10
  }
}

output openAiEndpoint string = openAi.properties.endpoint
output openAiKey string = openAi.listKeys().key1
