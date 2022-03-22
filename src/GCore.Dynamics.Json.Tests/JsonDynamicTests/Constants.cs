using System;

namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public static class Constants
{
    public const string TestModelSimpleJson = @"{
    ""menu"": {
        ""id"": ""file"",
        ""value"": ""File"",
        ""int_"": 42,
        ""float_"": 3.14159,
        ""bool_"": true,
        ""popup"": {
            ""menuitem"": [
                {
                    ""value"": ""New"",
                    ""onclick"": ""CreateNewDoc()""
                },
                {
                    ""value"": ""Open"",
                    ""onclick"": ""OpenDoc()""
                },
                {
                    ""value"": ""Close"",
                    ""onclick"": ""CloseDoc()""
                }
            ]
        }
    }
}";


    public static dynamic LoadTestModelSimple() =>
        DynamicJson.Deserialize(TestModelSimpleJson) ?? throw new NullReferenceException();
}