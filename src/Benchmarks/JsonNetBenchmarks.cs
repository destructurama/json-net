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

using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog;
using Destructurama;
using Destructurama.JsonNet;

namespace Benchmarks;

public class JsonNetBenchmarks
{
    private class HasName
    {
        public string? Name { get; set; }
    }

    private ILogEventPropertyValueFactory _factory = null!;
    private object _value = null!;
    private readonly JsonNetDestructuringPolicy _policy = new();

    [GlobalSetup]
    public void Setup()
    {
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
        _value = JsonConvert.DeserializeObject<dynamic>(ser)!;

        var log = new LoggerConfiguration()
            .Destructure.JsonNetTypes()
            .CreateLogger();

        var processor = log.GetType().GetField("_messageTemplateProcessor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(log)!;
        var converter = processor.GetType().GetField("_propertyValueConverter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(processor)!;
        _factory = (ILogEventPropertyValueFactory)converter;
    }

    [Benchmark]
    public void Destructure()
    {
        _policy.TryDestructure(_value, _factory, out _);
    }
}
