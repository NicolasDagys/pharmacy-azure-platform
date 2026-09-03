param appName string
param location string
param environmentId string
param managedIdentityResourceId string
param keyVaultUri string
param acrLoginServer string
param apiImage string
param appInsightsConnectionString string
param openAiEndpoint string

resource apiApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityResourceId}': {}
    }
  }

  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      registries: [
        {
          server: acrLoginServer
          identity: managedIdentityResourceId
        }
      ]
      ingress: {
        external: true
        targetPort: 8080
      }
      secrets: [
        {
          name: 'db-user'
          keyVaultUrl: '${keyVaultUri}secrets/db-user'
          identity: managedIdentityResourceId
        }
        {
          name: 'db-password'
          keyVaultUrl: '${keyVaultUri}secrets/db-password'
          identity: managedIdentityResourceId
        }
        {
          name: 'jwt-key'
          keyVaultUrl: '${keyVaultUri}secrets/jwt-key'
          identity: managedIdentityResourceId
        }
        {
          name: 'sql-connection-string'
          keyVaultUrl: '${keyVaultUri}secrets/sql-connection-string'
          identity: managedIdentityResourceId
        }
        {
          name: 'jwt-issuer'
          keyVaultUrl: '${keyVaultUri}secrets/jwt-issuer'
          identity: managedIdentityResourceId
        }
        {
          name: 'jwt-audience'
          keyVaultUrl: '${keyVaultUri}secrets/jwt-audience'
          identity: managedIdentityResourceId
        }
        /*{
          name: 'openai-endpoint'
          keyVaultUrl: '${keyVaultUri}secrets/openai-endpoint'
          identity: managedIdentityResourceId
        }*/
        {
          name: 'openai-api-key'
          keyVaultUrl: '${keyVaultUri}secrets/openai-api-key'
          identity: managedIdentityResourceId
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: apiImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }

          env: [
            {
              name: 'ConnectionStrings__PharmacyConnection'
              secretRef: 'sql-connection-string'
            }
            {
              name: 'Jwt__Key'
              secretRef: 'jwt-key'
            }
            {
              name: 'Jwt__Issuer'
              secretRef: 'jwt-issuer'
            }
            {
              name: 'Jwt__Audience'
              secretRef: 'jwt-audience'
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsightsConnectionString
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'OpenAI__Endpoint'
              value: openAiEndpoint
            }
            {
              name: 'OpenAI__Deployment'
              value: 'gpt-5-mini'
            }
            {
              name: 'OpenAI__ApiKey'
              secretRef: 'openai-api-key'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

output apiUrl string = 'https://${apiApp.properties.configuration.ingress.fqdn}'
