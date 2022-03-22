using System.Collections.Generic;
using GCore.Dynamics;
using NUnit.Framework;

namespace GCore.Dynamics.Tests;

public class DynamicToSimpleTests
{
    [Test]
    public void DynamicListToSimpleTest()
    {
        dynamic list = new DynamicList();
        list.Add(42);
        list.Add(0);
        list._1 = 43;

        var arr = list.ToSimpleObject();

        Assert.AreEqual(new object[]{42, 43}, arr);
    }

    [Test]
    public void DynamicDictToSimpleTest()
    {
        dynamic dict = new DynamicDict();
        dict["Test1"] = 42;
        dict.Test2 = 43;

        var d = dict.ToSimpleObject();

        Assert.AreEqual(new Dictionary<string, object?> { { "Test1", 42 }, { "Test2", 43} }, d);
    }
}