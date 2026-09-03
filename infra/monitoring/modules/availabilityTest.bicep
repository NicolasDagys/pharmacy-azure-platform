param availabilityTestName string
param appInsightsId string
param location string
param endpointUrl string//es usado en WebTest pero me aparece en amarillo. lo estara tomando??
var webTestTemplate = '''<WebTest Name="@@NAME@@" Enabled="True">
  <Items>
    <Request Method="GET" Url="@@URL@@" ParseDependentRequests="True" FollowRedirects="True" />
  </Items>
</WebTest>'''

resource availabilityTest 'Microsoft.Insights/webtests@2022-06-15' = {
name: availabilityTestName
location: location
kind: 'ping'

tags: {
'hidden-link:${appInsightsId}': 'Resource'
}

properties: {
SyntheticMonitorId: availabilityTestName
Name: availabilityTestName
Description: 'Health endpoint availability test'
Enabled: true


Frequency: 300
Timeout: 30
RetryEnabled: true
Kind: 'ping'

Locations: [
  {
    Id: 'us-ca-sjc-azr'
  }
  {
    Id: 'us-tx-sn1-azr'
  }
  {
    Id: 'emea-nl-ams-azr'
  }
]

Configuration: {
  WebTest: replace(replace(webTestTemplate, '@@NAME@@', availabilityTestName), '@@URL@@', endpointUrl)
}
  }
}

output testId string = availabilityTest.id
