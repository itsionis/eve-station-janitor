# Eve Station Janitor

## Overview

This tool scans Eve Online trade hubs for items that can be bought, reprocessed and sold back to the market for a profit.

Persisted data and logs can be found in `%LOCALAPPDATA%\Eve Station Janitor`

### Requirements

* [.NET 8 SDK x64](https://dotnet.microsoft.com/en-us/download)

## Usage

Run directly with `dotnet`

```pwsh
cd EveStationJanitor
dotnet run --configuration Release
```

Or run the published binary

```pwsh
.\Scripts\publish.ps1
cd .\EveStationJanitor\bin\Release\net8.0-window\win-x64\
.\EveStationJanitor.exe
```
