using System.Collections.Generic;
using System.Linq;
using Destructurama.JsonNet.Tests.Support;
using Newtonsoft.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

namespace Destructurama.JsonNet.Tests
{
    class HasName
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class JsonNetTypesDestructuringTests
    {
        [Test]
        public void AttributesAreConsultedWhenDestructuring()
        {
            LogEvent evt = null;

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
                E = (object)null
            };

            var ser = JsonConvert.SerializeObject(test, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var dyn = JsonConvert.DeserializeObject<dynamic>(ser);

            log.Information("Here is {@Dyn}", dyn);

            var sv = (StructureValue)evt.Properties["Dyn"];
            var props = sv.Properties.ToDictionary(p => p.Name, p => p.Value);

            Assert.IsInstanceOf<StructureValue>(props["HN"]);
            Assert.IsInstanceOf<SequenceValue>(props["Arr"]);
            Assert.IsInstanceOf<string>(props["S"].LiteralValue());
            Assert.IsNull(props["E"].LiteralValue());

            // Not currently handled correctly - will serialize as a structure
            // Assert.IsInstanceOf<DictionaryValue>(props["D"]);
        }
    }
}
