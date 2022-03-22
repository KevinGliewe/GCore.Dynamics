using NUnit.Framework;

namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public class DynamicTests : BaseTest
{
    [Test]
    public void Dynamic()
    {
        dynamic d = Json;

        Assert.AreEqual("file", d.menu.id);
        Assert.AreEqual(42, d.menu.int_);
        Assert.AreEqual(3.14159, d.menu.float_);
        Assert.AreEqual(true, d.menu.bool_);
        Assert.AreEqual("Open", d.menu.popup.menuitem[1].value);
    }
}