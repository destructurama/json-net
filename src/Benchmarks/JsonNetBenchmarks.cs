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
using Destructurama;
using Destructurama.JsonNet;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace Benchmarks;

public class JsonNetBenchmarks
{
    private class HasName
    {
        public string? Name { get; set; }
    }

    private ILogEventPropertyValueFactory _factory = null!;
    private IDestructuringPolicy _policy = null!;
    private object _value = null!;

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

        (_policy, _factory) = Build(c => c.Destructure.JsonNetTypes());
    }

    private static (IDestructuringPolicy, ILogEventPropertyValueFactory) Build(Func<LoggerConfiguration, LoggerConfiguration> configure)
    {
        var configuration = new LoggerConfiguration();
        var logger = configure(configuration).CreateLogger();

        var processor = logger.GetType().GetField("_messageTemplateProcessor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(logger)!;
        var converter = processor.GetType().GetField("_propertyValueConverter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(processor)!;
        var factory = (ILogEventPropertyValueFactory)converter;
        var policies = (IDestructuringPolicy[])converter.GetType().GetField("_destructuringPolicies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(converter)!;
        var policy = policies.First(p => p is JsonNetDestructuringPolicy);
        return (policy, factory);
    }

    [Benchmark]
    public void Destructure()
    {
        _policy.TryDestructure(_value, _factory, out _);
    }
}
