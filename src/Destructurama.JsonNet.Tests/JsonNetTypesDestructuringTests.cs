// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
    [Theory]
    [InlineData(TypeNameHandling.Auto)]
    [InlineData(TypeNameHandling.Objects)]
    public void AttributesAreConsultedWhenDestructuring(TypeNameHandling typeNameHandling)
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
            TypeNameHandling = typeNameHandling
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

        foreach (var value in props.Values.OfType<StructureValue>())
        {
            if (typeNameHandling == TypeNameHandling.Auto)
                value.TypeTag.ShouldBeNull();
            else if (typeNameHandling == TypeNameHandling.Objects)
                value.TypeTag.ShouldNotBeNull();
        }
    }

    [Fact]
    public void TryDestructure_Should_Return_False_When_Called_With_Null()
    {
        var policy = new JsonNetDestructuringPolicy();
        policy.TryDestructure(null!, null!, out _).ShouldBeFalse();
    }
}
