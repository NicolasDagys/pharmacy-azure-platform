param alertName string
param appInsightsId string
param actionGroupId string
param availabilityTestId string

resource availabilityAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: alertName
  location: 'global'
  properties: {
    description: 'Availability test failed'
    severity: 2
    enabled: true
    scopes: [
      appInsightsId
    ]

    evaluationFrequency: 'PT5M'
    windowSize: 'PT5M'

    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
      webTestId: availabilityTestId
      componentId: appInsightsId
      failedLocationCount: 2
    }

    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

output alertId string = availabilityAlert.id
