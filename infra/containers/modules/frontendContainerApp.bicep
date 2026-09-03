param appName string
param location string
param environmentId string
param frontendImage string
param acrLoginServer string
param managedIdentityResourceId string
param apiUrl string

resource frontendApp 'Microsoft.App/containerApps@2024-03-01' = {
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
      ingress: {
        external: true
        targetPort: 80
        transport: 'Auto'
      }

      registries: [
        {
          server: acrLoginServer
          identity: managedIdentityResourceId
        }
      ]
    }

    template: {
      containers: [
        {
          name: 'frontend'
          image: frontendImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'API_URL'
              value: apiUrl
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

output frontendUrl string = 'https://${frontendApp.properties.configuration.ingress.fqdn}'
