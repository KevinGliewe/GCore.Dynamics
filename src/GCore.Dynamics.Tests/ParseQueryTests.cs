using GCore.Dynamics;
using GCore.Dynamics.Traits;
using NUnit.Framework;

namespace GCore.Dynamics.Tests;

public class ParseQueryTests
{
    public static void AssertAccessor(IQueryable.IAccessor expected, IQueryable.IAccessor actual)
    {
        if (expected is IQueryable.MemberAccessor exm)
            Assert.AreEqual(exm.Name, ((IQueryable.MemberAccessor)actual).Name);
        else if (expected is IQueryable.IndexAccessor exi)
            Assert.AreEqual(exi.Index, ((IQueryable.IndexAccessor)actual).Index);
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void ParseMember()
    {
        var q = new IQueryable.AccessorQuery("asd.fgh");

        Assert.AreEqual(2, q.Query.Count);
        AssertAccessor(q.Query[0], new IQueryable.MemberAccessor("asd"));
        AssertAccessor(q.Query[1], new IQueryable.MemberAccessor("fgh"));
    }

    [Test]
    public void ParseIndex()
    {
        var q = new IQueryable.AccessorQuery("[42]");

        Assert.AreEqual(1, q.Query.Count);
        AssertAccessor(q.Query[0], new IQueryable.IndexAccessor(42));

    }

    [Test]
    public void ParseMultibleIndex()
    {
        var q = new IQueryable.AccessorQuery("[42][43]");

        Assert.AreEqual(2, q.Query.Count);
        AssertAccessor(q.Query[0], new IQueryable.IndexAccessor(42));
        AssertAccessor(q.Query[1], new IQueryable.IndexAccessor(43));

    }

    [Test]
    public void ParseComplex()
    {
        var q = new IQueryable.AccessorQuery("asd[0].fgh[0][42]");
    }
}