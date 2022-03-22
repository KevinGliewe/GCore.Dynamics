using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using GCore.Dynamics.Extensions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GCore.Dynamics.Tests;

public class FromSimpleTests
{
    public static readonly int[] IntArray = {0, 1, 2, 3};

    public static void AssertIntArray(dynamic d)
    {
        Assert.AreEqual(4, d.Count);
        Assert.AreEqual(IntArray[0], d[0]);
        Assert.AreEqual(IntArray[1], d[1]);
        Assert.AreEqual(IntArray[2], d[2]);
        Assert.AreEqual(IntArray[3], d[3]);
    }

    public static readonly object?[] MiscArray = new object?[]
    {
        null,
        IntArray,
        42,
        "Test",
    };

    public static void AssertMiscArray(dynamic d)
    {
        Assert.AreEqual(4, d.Count);
        Assert.AreEqual(MiscArray[0], d[0]);
        AssertIntArray(d[1]);
        Assert.AreEqual(MiscArray[2], d[2]);
        Assert.AreEqual(MiscArray[3], d[3]);
    }

    public static readonly Dictionary<string, object?> MiscDictionary = new Dictionary<string, object?>()
    {
        {"_null", null},
        {"_intarr", IntArray},
        {"_int", 42},
        {"_String", "Test"},
        {"_MiscArray", MiscArray},
    };

    public static void AssertMiscDictionary(dynamic d)
    {
        Assert.AreEqual(MiscDictionary["_null"], d._null);
        AssertIntArray(d._intarray);
        Assert.AreEqual(MiscDictionary["_int"], d._int);
        Assert.AreEqual(MiscDictionary["_String"], d._String);
        AssertMiscArray(d._MiscArray);
    }

    [Test]
    public void IntArrayConvert()
    {
        var d = IntArray.ToDynamic() ?? throw new NullReferenceException();

        AssertIntArray(d);
    }


    [Test]
    public void MiscArrayConvert()
    {
        var d = MiscArray.ToDynamic() ?? throw new NullReferenceException();

        AssertMiscArray(d);
    }

    [Test]
    public void MiscDictionaryTest()
    {

    }
}