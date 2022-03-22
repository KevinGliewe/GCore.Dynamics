using System.IO;
using GCore.Dynamics.Json;
using NUnit.Framework;

namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public class QueryTests : BaseTest
{


    [Test]
    public void Query()
    {
        Assert.AreEqual("file", Json.Query("menu.id"));
        Assert.AreEqual(42, Json.Query("menu.int_"));
        Assert.AreEqual(3.14159, Json.Query("menu.float_"));
        Assert.AreEqual(true, Json.Query("menu.bool_"));
        Assert.AreEqual("Open", Json.Query("menu.popup.menuitem[1].value"));
    }


    //[Test]
    //public void IndexString()
    //{
    //    Assert.AreEqual("file", Json["menu.id"]);
    //    Assert.AreEqual("file", Json["menu"]?["id"]);
    //}
}