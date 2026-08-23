targetScope = 'resourceGroup'

param environment string
param actionGroupEmail string


module logAnalytics './modules/logAnalytics.bicep' = {
name: 'logAnalytics'

params: {
workspaceName: 'ob-law-${environment}-wus3'
location: resourceGroup().location
}
}

module appInsights './modules/applicationInsights.bicep' = {
name: 'applicationInsights'

params: {
appInsightsName: 'ob-law-${environment}-wus3'
location: resourceGroup().location
workspaceResourceId: logAnalytics.outputs.workspaceId
}
}

module actionGroup './modules/actionGroup.bicep' = {
name: 'actionGroup'
params: {
actionGroupName: 'ob-ag-${environment}-global'
emailAddress: actionGroupEmail
}
}

/*module availabilityTest './modules/availabilityTest.bicep' = {
name: 'availabilityTest'

params: {
availabilityTestName: 'ob-avt-health-${environment}'
appInsightsId: appInsights.outputs.appInsightsId
location: resourceGroup().location
endpointUrl: '${apiUrl}/api/diagnostics/ping'
}
}*/

/*module alerts './modules/alerts.bicep' = {
name: 'alerts'

params: {
appInsightsId: appInsights.outputs.appInsightsId
actionGroupId: actionGroup.outputs.actionGroupId
alertName: 'ob-alt-availability-${environment}'
}
}*/

output appInsightsConnectionString string = appInsights.outputs.appInsightsConnectionString
output appInsightsId string = appInsights.outputs.appInsightsId
output workspaceId string = logAnalytics.outputs.workspaceId
output actionGroupId string = actionGroup.outputs.actionGroupId
