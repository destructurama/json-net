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
