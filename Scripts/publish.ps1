$ScriptDir = $PSScriptRoot

Push-Location $ScriptDir/..

dotnet publish EveStationJanitor `
    --runtime win-x64 `
    --configuration Release `
    --self-contained `
    --verbosity normal

Pop-Location
