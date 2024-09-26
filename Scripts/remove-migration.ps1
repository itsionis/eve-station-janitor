$ScriptDir = $PSScriptRoot

Push-Location $ScriptDir/..

dotnet ef migrations remove `
    --project .\EveStationJanitor.Core `
    --startup-project .\EveStationJanitor `
    --verbose `

Pop-Location