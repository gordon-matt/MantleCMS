param(
	[Parameter(Mandatory = $true)]
	[string]$ApiKey,
	
	[Parameter(Mandatory = $true)]
	[string]$Version
)

$Source = "https://api.nuget.org/v3/index.json"

$Projects = @(
    "Mantle.Data.Abstractions"
    "Mantle.Infrastructure.Abstractions"
    "Mantle.Tenants.Abstractions"
    "Mantle.Localization.Abstractions"
    "Mantle.Caching.Abstractions"
    "Mantle.Events.Abstractions"
    "Mantle.Tasks.Abstractions"
    "Mantle"
    "Mantle.Security"
    "Mantle.Web.Abstractions"
    "Mantle.Plugins"
    "Mantle.Caching"
    "Mantle.Data"
    "Mantle.Data.Dapper"
    "Mantle.Data.Dapper.PostgreSql"
    "Mantle.Events"
    "Mantle.Infrastructure"
    "Mantle.Infrastructure.Autofac"
    "Mantle.Infrastructure.DotNetDefault"
    "Mantle.Infrastructure.DryIoc"
    "Mantle.Infrastructure.Lamar"
    "Mantle.Infrastructure.LightInject"
    "Mantle.Localization"
    "Mantle.Logging"
    "Mantle.Messaging"
    "Mantle.Tasks"
    "Mantle.Tenants"
    "Mantle.Web"
    "Mantle.Identity"
    "Mantle.Web.Common"
    "Mantle.Web.CommonResources"
    "Mantle.Web.Messaging"
    "Mantle.Web.ContentManagement"
    "Mantle.Web.Infrastructure.Autofac"
    "Mantle.Web.Infrastructure.DotNetDefault"
    "Mantle.Web.Infrastructure.DryIoc"
    "Mantle.Web.Infrastructure.Lamar"
    "Mantle.Web.Infrastructure.LightInject"
)

foreach ($ProjectDir in $Projects) {
    $Name = Split-Path $ProjectDir -Leaf
    $Project = Join-Path $ProjectDir "$Name.csproj"
    $ProjectText = Get-Content $Project -Raw
    if ($ProjectText -match '<PackageId>([^<]+)</PackageId>') {
        $PackageId = $Matches[1]
    }
    else {
        $PackageId = $Name
    }
    $PackageFile = Join-Path $ProjectDir "bin\Release\$PackageId.$Version.nupkg"

    Write-Host "`nPacking $PackageId..."
    dotnet pack $Project -c Release -p:PackageVersion=$Version

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to pack $PackageId"
    }

    if (-not (Test-Path $PackageFile)) {
        throw "Package not found: $PackageFile"
    }

    Write-Host "Pushing $PackageFile..."

    dotnet nuget push $PackageFile --api-key $ApiKey --source $Source

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push $PackageId"
    }
}

Write-Host "`nAll packages packed and pushed successfully!"
