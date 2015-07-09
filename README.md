### Destructurama.JsonNet

[![Build status](https://ci.appveyor.com/api/projects/status/mkvloyvlwdbky28r/branch/master?svg=true)](https://ci.appveyor.com/project/Destructurama/json-net/branch/master)

Adds support for logging JSON.NET dynamic types as structured data with Serilog.

#### Enabling the module:

Install from NuGet:

```powershell
Install-Package Destructurama.JsonNet
```

Modify logger configuration:

```csharp
var log = new LoggerConfiguration()
  .Destructure.JsonNetTypes()
  ...
```

#### Logging

Any JSON.NET dynamic object can be represented in the log event's properties:

```csharp
var obj = JsonConvert.DeserializeObject<dynamic>(someJson);
Log.Information("Deserialized {@Obj}", obj);
```

