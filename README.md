# Destructurama.JsonNet

![License](https://img.shields.io/github/license/destructurama/json-net)

[![codecov](https://codecov.io/gh/destructurama/json-net/graph/badge.svg?token=abGh9D57gU)](https://codecov.io/gh/destructurama/json-net)
[![Nuget](https://img.shields.io/nuget/dt/Destructurama.JsonNet)](https://www.nuget.org/packages/Destructurama.JsonNet)
[![Nuget](https://img.shields.io/nuget/v/Destructurama.JsonNet)](https://www.nuget.org/packages/Destructurama.JsonNet)

[![GitHub Release Date](https://img.shields.io/github/release-date/destructurama/json-net?label=released)](https://github.com/destructurama/json-net/releases)
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/destructurama/json-net/latest?label=new+commits)](https://github.com/destructurama/json-net/commits/master)
![Size](https://img.shields.io/github/repo-size/destructurama/json-net)

[![GitHub contributors](https://img.shields.io/github/contributors/destructurama/json-net)](https://github.com/destructurama/json-net/graphs/contributors)
![Activity](https://img.shields.io/github/commit-activity/w/destructurama/json-net)
![Activity](https://img.shields.io/github/commit-activity/m/destructurama/json-net)
![Activity](https://img.shields.io/github/commit-activity/y/destructurama/json-net)

[![Run unit tests](https://github.com/destructurama/json-net/actions/workflows/test.yml/badge.svg)](https://github.com/destructurama/json-net/actions/workflows/test.yml)
[![Publish preview to GitHub registry](https://github.com/destructurama/json-net/actions/workflows/publish-preview.yml/badge.svg)](https://github.com/destructurama/json-net/actions/workflows/publish-preview.yml)
[![Publish release to Nuget registry](https://github.com/destructurama/json-net/actions/workflows/publish-release.yml/badge.svg)](https://github.com/destructurama/json-net/actions/workflows/publish-release.yml)
[![CodeQL analysis](https://github.com/destructurama/json-net/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/destructurama/json-net/actions/workflows/codeql-analysis.yml)

Adds support for logging JSON.NET dynamic types as structured data with [Serilog](https://serilog.net).
For System.Text.Json dynamic types see [this repo](https://github.com/destructurama/system-text-json).

# Installation

Install from NuGet:

```powershell
Install-Package Destructurama.JsonNet
```

# Usage

Modify logger configuration:

```csharp
var log = new LoggerConfiguration().Destructure.JsonNetTypes()
```

Now any JSON.NET dynamic object can be represented in the log event's properties:

```csharp
using Destructurama;
using Newtonsoft.Json;
using Serilog;

var logger1 = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var logger2 = new LoggerConfiguration().Destructure.JsonNetTypes().WriteTo.Console().CreateLogger();

var json = """
    {
      "name": "Tom",
      "age": 42,
      "isDeveloper": true
    }
    """;

var obj = JsonConvert.DeserializeObject<dynamic>(json);

logger1.Information("Deserialized without JsonNetTypes(): {@Obj}", obj);

logger2.Information("Deserialized with JsonNetTypes(): {@Obj}", obj);

Console.ReadKey();
```

Output:

```
[20:27:59 INF] Deserialized without JsonNetTypes(): [[[]], [[]], [[]]]
[20:27:59 INF] Deserialized with JsonNetTypes(): {"name": "Tom", "age": 42, "isDeveloper": true}
```

# Benchmarks

The results are available [here](https://destructurama.github.io/json-net/dev/bench/).
