$ScriptDir = $PSScriptRoot

Push-Location $ScriptDir/..

dotnet publish `
    --runtime win-x64 `
    --configuration Release `
    --self-contained `
    --verbosity normal

Pop-Location
