[CmdletBinding()]
Param (
    [Parameter(Mandatory=$true)]
    [string]$Name
)

$ScriptDir = $PSScriptRoot

Push-Location $ScriptDir/..

dotnet ef migrations add `
    --project .\EveStationJanitor.Core `
    --startup-project .\EveStationJanitor `
    --output-dir DataAccess\Migrations `
    --verbose `
    $Name

Pop-Location