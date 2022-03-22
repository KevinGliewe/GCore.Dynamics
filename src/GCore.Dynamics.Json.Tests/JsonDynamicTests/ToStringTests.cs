using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using NUnit.Framework;

namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public class ToStringTests : BaseTest
{
    [Test]
    public void ToStringTest()
    {
        string str = DynamicJson.Serialise(Json, new JsonSerializerOptions() {WriteIndented = true});
        File.WriteAllText("test.json", str);
        
        Assert.AreEqual(Constants.TestModelSimpleJson, str.Replace("  ", "    "));
    }
}