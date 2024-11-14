# Eve Station Janitor

## Overview

This tool scans Eve Online trade hubs for items that can be bought, reprocessed and sold back to the market for a profit.

Persisted data and logs can be found in `%LOCALAPPDATA%\Eve Station Janitor`

### Requirements

* [.NET 9 SDK x64](https://dotnet.microsoft.com/en-us/download)

## Usage

Run directly with `dotnet`

```pwsh
dotnet build --configuration Release
cd EveStationJanitor\bin\Release\net9.0-windows
.\EveStationJanitor.exe
```

Or run the published binary

```pwsh
.\Scripts\publish.ps1
cd .\EveStationJanitor\bin\Release\net9.0-window\win-x64\
.\EveStationJanitor.exe
```
