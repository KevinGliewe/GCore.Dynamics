using System;
using GCore.Dynamics;
using GCore.Dynamics.Extensions;
using GCore.Dynamics.Traits;
using NUnit.Framework;

namespace GCore.Dynamics.Tests;

public class AccessQueryTests
{
    [Test]
    public void AccessArray()
    {
        var q = new IQueryable.AccessorQuery("[1]");

        var result = q.Access(FromSimpleTests.IntArray);

        Assert.AreEqual(FromSimpleTests.IntArray[1], result);
    }

    [Test]
    public void AccessMap()
    {
        var q = new IQueryable.AccessorQuery("_int");

        var result = q.Access(FromSimpleTests.MiscDictionary);

        Assert.AreEqual(FromSimpleTests.MiscDictionary["_int"], result);
    }

    [Test]
    public void AccessMapArray()
    {
        var q = new IQueryable.AccessorQuery("_intarr[1]");

        var result = q.Access(FromSimpleTests.MiscDictionary);

        Assert.AreEqual(FromSimpleTests.IntArray[1], result);
    }

    [Test]
    public void AccessReflect()
    {
        var q = new IQueryable.AccessorQuery("TestMember");

        var result = q.Access(new { TestMember = 42 });

        Assert.AreEqual(42, result);
    }

    [Test]
    public void AccessDynamicListTest()
    {
        var q = new IQueryable.AccessorQuery("[1]");

        var result = q.Access(FromSimpleTests.IntArray.ToDynamic());

        Assert.AreEqual(FromSimpleTests.IntArray[1], result);
    }

    [Test]
    public void AccessDynamicDict()
    {
        var q = new IQueryable.AccessorQuery("_int");

        var result = q.Access(FromSimpleTests.MiscDictionary.ToDynamic());

        Assert.AreEqual(FromSimpleTests.MiscDictionary["_int"], result);
    }

    [Test]
    public void AccessComplex()
    {
        var q = new IQueryable.AccessorQuery("TestMember._intarr[1]");

        var result = q.Access(new { TestMember = FromSimpleTests.MiscDictionary });

        Assert.AreEqual(FromSimpleTests.IntArray[1], result);
    }

    [Test]
    public void AccessDynamicComplex()
    {
        var q = new IQueryable.AccessorQuery("TestMember._intarr[1]");

        var result = q.Access(new { TestMember = FromSimpleTests.MiscDictionary }.ToDynamic());

        Assert.AreEqual(FromSimpleTests.IntArray[1], result);
    }

    [Test]
    public void AccessFail()
    {
        var q = new IQueryable.AccessorQuery("DoesNotExist");

        Assert.Throws<IQueryable.AccessorMismatch>(() =>
        {
            q.Access(FromSimpleTests.MiscDictionary);
        });

    }

    [Test]
    public void AccessNull()
    {
        var q = new IQueryable.AccessorQuery("DoesNotExist");

        var result = q.AccessNull(FromSimpleTests.MiscDictionary);

        Assert.AreEqual(null, result);
    }
}