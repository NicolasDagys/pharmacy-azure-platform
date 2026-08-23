targetScope = 'resourceGroup'
param environment string
param apiUrl string
param appInsightsId string
param actionGroupId string

module availabilityTest './modules/availabilityTest.bicep' = {
  name: 'availabilityTest'
  params: {
    availabilityTestName: 'ob-avt-health-${environment}'
    appInsightsId: appInsightsId
    location: resourceGroup().location
    endpointUrl: '${apiUrl}/api/diagnostics/ping'
  }
}

module alerts './modules/alerts.bicep' = {
  name: 'alerts'
  params: {
    appInsightsId: appInsightsId
    actionGroupId: actionGroupId
    availabilityTestId: availabilityTest.outputs.testId 
    alertName: 'ob-alt-availability-${environment}'
  }
}

output availabilityTestId string = availabilityTest.outputs.testId
