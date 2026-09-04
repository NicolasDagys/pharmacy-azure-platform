$ErrorActionPreference = "Stop"

$Root = $PSScriptRoot
$ParametersFile = Join-Path $Root "infra\main.parameters.json"
$json = Get-Content $ParametersFile -Raw | ConvertFrom-Json

$EnvFile = Join-Path $Root ".env"

if (Test-Path $EnvFile) {

    Get-Content $EnvFile | ForEach-Object {

        if ($_ -match '^\s*([^#][^=]*)=(.*)$') {

            $name = $matches[1].Trim()
            $value = $matches[2].Trim()

            [Environment]::SetEnvironmentVariable(
                $name,
                $value,
                "Process"
            )
        }
    }
}

$Location          = $json.parameters.location.value
$ResourceGroup     = $json.parameters.resourceGroupName.value
$Environment       = $json.parameters.environment.value
$SqlAdminUser      = $json.parameters.sqlAdminUser.value
#$AdminPassword     = $json.parameters.administratorPassword.value
#$jwtKey            = $json.parameters.jwtKey.value
$jwtIssuer         = $json.parameters.jwtIssuer.value
$jwtAudience       = $json.parameters.jwtAudience.value
$actionGroupEmail  = $json.parameters.actionGroupEmail.value
#$ApiImage          = $json.parameters.apiImage.value
#$FrontendImage     = $json.parameters.frontendImage.value

#ESTA ES LA PARTE QUE AGREGAMOS:
$AdminPassword = $env:ADMINISTRATOR_PASSWORD
$JwtKey        = $env:JWT_KEY

if ([string]::IsNullOrWhiteSpace($AdminPassword)) {
    throw "ADMINISTRATOR_PASSWORD environment variable is not set."
}

if ([string]::IsNullOrWhiteSpace($JwtKey)) {
    throw "JWT_KEY environment variable is not set."
}
$ApiImage      = "pharmacy-api:$Environment"
$FrontendImage = "pharmacy-frontend:$Environment"

#HASTA ACA

Write-Host "Getting public IP..."
$PublicIp = (Invoke-RestMethod -Uri "https://api.ipify.org").Trim()
Write-Host "Current IP: $PublicIp"

# ==========================================
# FASE 1: Infra base (sin imágenes)
# ==========================================

Write-Host ""
Write-Host "Deploying base infrastructure..."

$OldErrorAction = $ErrorActionPreference
$ErrorActionPreference = "Continue"

# Guardamos el JSON en bruto sin procesarlo todavía, usando 2>$null para omitir warnings
$deploymentRawJson = az deployment sub create `
    --location $Location `
    --template-file "$Root\infra\main.bicep" `
    --parameters location=$Location `
    --parameters resourceGroupName=$ResourceGroup `
    --parameters environment=$Environment `
    --parameters sqlAdminUser=$SqlAdminUser `
    --parameters administratorPassword=$AdminPassword `
    --parameters jwtKey=$JwtKey `
    --parameters jwtIssuer=$jwtIssuer `
    --parameters jwtAudience=$jwtAudience `
    --parameters actionGroupEmail=$actionGroupEmail `
    --parameters currentPublicIp=$PublicIp `
    --output json 2>$null

$ErrorActionPreference = $OldErrorAction

# Validamos de manera inteligente el estado real de Azure
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Azure CLI error: El despliegue de infraestructura falló de forma real."
    throw "Infrastructure deployment failed."
}

# Convertimos a JSON una ÚNICA VEZ ahora que sabemos que el comando fue exitoso
$deployment = $deploymentRawJson | ConvertFrom-Json

$SqlServerName               = $deployment.properties.outputs.sqlServerName.value
$DatabaseName                = $deployment.properties.outputs.databaseName.value
$KeyVaultUri                 = $deployment.properties.outputs.keyVaultUri.value
$ManagedIdentityResourceId   = $deployment.properties.outputs.managedIdentityResourceId.value
$AppInsightsConnectionString = $deployment.properties.outputs.appInsightsConnectionString.value
$AcrLoginServer              = $deployment.properties.outputs.acrLoginServer.value
$ContainerAppEnvironmentId   = $deployment.properties.outputs.containerAppEnvironmentId.value
$OpenAiEndpoint = $deployment.properties.outputs.openAiEndpoint.value

# ==========================================
# FASE 2: Build imágenes (ACR ya existe)
# ==========================================
Write-Host ""
Write-Host "Building API image in Azure..."

az acr build `
    --registry "pharmacyacr$Environment" `
    --image $ApiImage `
    --file "$Root\src\PharmacyApiEF\Dockerfile" `
    "$Root\src\PharmacyApiEF"

if ($LASTEXITCODE -ne 0) {
    throw "ACR API image build failed."
}

Write-Host ""
Write-Host "Building Frontend image in Azure..."

az acr build `
    --registry "pharmacyacr$Environment" `
    --image $FrontendImage `
    --file "$Root\src\PharmacyAngular\Dockerfile" `
    "$Root\src\PharmacyAngular"

if ($LASTEXITCODE -ne 0) {
    throw "ACR Frontend image build failed."
}

# ==========================================
# FASE 3: Inicializar base de datos (ANTES de crear los containers)
# ==========================================
Write-Host ""
Write-Host "Waiting for SQL firewall propagation..."
Start-Sleep -Seconds 30

Write-Host ""
Write-Host "Initializing database..."

powershell.exe `
    -ExecutionPolicy Bypass `
    -File "$Root\database\deploy\InitializeDatabase.ps1" `
    -ServerName $SqlServerName `
    -DatabaseName $DatabaseName `
    -AdminUser $SqlAdminUser `
    -AdminPassword $AdminPassword

if ($LASTEXITCODE -ne 0) {
    throw "Database initialization failed."
}


# ==========================================
# FASE 4: Deploy Container Apps (imágenes ya existen)
# ==========================================
Write-Host ""
Write-Host "Deploying Container Apps..."

$containersDeployment = az deployment group create `
    --resource-group $ResourceGroup `
    --template-file "$Root\infra\containers\main.bicep" `
    --parameters environment=$Environment `
    --parameters apiImage="$AcrLoginServer/$ApiImage" `
    --parameters frontendImage="$AcrLoginServer/$FrontendImage" `
    --parameters environmentId=$ContainerAppEnvironmentId `
    --parameters acrLoginServer=$AcrLoginServer `
    --parameters keyVaultUri=$KeyVaultUri `
    --parameters managedIdentityResourceId=$ManagedIdentityResourceId `
    --parameters appInsightsConnectionString=$AppInsightsConnectionString `
    --parameters openAiEndpoint="$OpenAiEndpoint" `
    -o json | ConvertFrom-Json

if ($LASTEXITCODE -ne 0) {
    throw "Container Apps deployment failed."
}

$FrontendUrl = $containersDeployment.properties.outputs.frontendUrl.value
$ApiUrl      = $containersDeployment.properties.outputs.apiUrl.value

Write-Host ""
Write-Host "=========================================="
Write-Host "Deployment completed successfully"
Write-Host "=========================================="
Write-Host "API URL:      $ApiUrl"
Write-Host "Frontend URL: $FrontendUrl"