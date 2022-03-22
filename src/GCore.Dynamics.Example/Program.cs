using System.Text.Json;
using GCore.Dynamics.Extensions;
using GCore.Dynamics.Json;

var TestModelSimpleJson = @"[
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
    ]";

var d = DynamicJson.Deserialize(TestModelSimpleJson) ?? throw new NullReferenceException();

d[0].value = "NewNew";
d.Add(new int[] {1, 2, 3});

Console.WriteLine(d.ToString());

Console.WriteLine(DynamicJson.Serialise(d, new JsonSerializerOptions() {WriteIndented = true}));