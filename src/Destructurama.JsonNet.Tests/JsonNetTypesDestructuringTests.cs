using Destructurama.JsonNet.Tests.Support;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Shouldly;
using Xunit;

namespace Destructurama.JsonNet.Tests;

internal class HasName
{
    public string? Name { get; set; }
}

public class JsonNetTypesDestructuringTests
{
    [Fact]
    public void AttributesAreConsultedWhenDestructuring()
    {
        LogEvent evt = null!;

        var log = new LoggerConfiguration()
            .Destructure.JsonNetTypes()
            .WriteTo.Sink(new DelegatingSink(e => evt = e))
            .CreateLogger();

        var test = new
        {
            HN = new HasName { Name = "Some name" },
            Arr = new[] { 1, 2, 3 },
            S = "Some string",
            D = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } },
            E = (object?)null,
            ESPN = JsonConvert.DeserializeObject("{\"\":\"Empty string property name\"}"),
            WSPN = JsonConvert.DeserializeObject("{\"\r\n\":\"Whitespace property name\"}")
        };

        string ser = JsonConvert.SerializeObject(test, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        var dyn = JsonConvert.DeserializeObject<dynamic>(ser);

        log.Information("Here is {@Dyn}", dyn);

        var sv = (StructureValue)evt.Properties["Dyn"];
        var props = sv.Properties.ToDictionary(p => p.Name, p => p.Value);

        props["HN"].ShouldBeOfType<StructureValue>();
        props["Arr"].ShouldBeOfType<SequenceValue>();
        props["S"].LiteralValue().ShouldBeOfType<string>();
        props["D"].ShouldBeOfType<StructureValue>();
        props["E"].LiteralValue().ShouldBeNull();
        props["ESPN"].ShouldBeOfType<DictionaryValue>();
        props["WSPN"].ShouldBeOfType<DictionaryValue>();
    }
}
