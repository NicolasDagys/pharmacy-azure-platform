param actionGroupName string
param emailAddress string
param shortName string = 'PHARM'

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'Global'
  properties: {
    groupShortName: shortName
    enabled: true

    emailReceivers: [
      {
        name: 'AdminEmail'
        emailAddress: emailAddress
        useCommonAlertSchema: true
      }
    ]
  }
}

output actionGroupId string = actionGroup.id
output actionGroupName string = actionGroup.name
