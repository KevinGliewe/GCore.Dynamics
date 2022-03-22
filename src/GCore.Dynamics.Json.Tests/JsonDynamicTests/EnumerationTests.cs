using NUnit.Framework;

namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public class EnumerationTests : BaseTest
{
    [Test]
    public void EnumerateArray()
    {
        dynamic? d = Json.Query("menu.popup.menuitem");
        int index = 0;
        var vals = new string[] { "New", "Open", "Close" };

        if (d != null)
            foreach (dynamic? item in d)
            {
                Assert.AreEqual(vals[index], item.value);
                index++;
            }
        else
            Assert.Fail();
    }

    [Test]
    public void EnumerateObject()
    {
        dynamic? d = Json.Query("menu");
        int index = 0;
        var vals = new string[] { "id", "value", "int_", "float_", "bool_", "popup" };

        if (d != null)
            foreach (dynamic? item in d)
            {
                Assert.AreEqual(vals[index], item.Key);
                index++;
            }
        else
            Assert.Fail();
    }
}