using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;
using GCore.Dynamics.Extensions;

namespace GCore.Dynamics;

public class DynamicList : DynamicObject, IList<object?>, IReadOnlyable, ISimplifyable, IQueryable
{
    protected Regex IndexMemberRegex = new Regex(@"^_(?<index>\d+)$");
    protected IList<object?> _list = new List<object?>();



    public static DynamicList FromEnumerable(IEnumerable enumerable)
    {
        var list = new List<object?>();
        foreach (var item in enumerable)
            list.Add(item.ToDynamic());
        return new DynamicList(list);
    }

    public DynamicList() {}

    public DynamicList(IEnumerable enumerable)
    {
        var list = new List<object?>();
        foreach (var item in enumerable)
            list.Add(item);
        _list = list;
    }

    public DynamicList(IList<object?> list)
    {
        _list = list;
    }

    #region IReadOnlyable
    public void SetReadOnly(bool onlyread, bool recursive = true)
    {
        IsReadOnly = onlyread;

        if (recursive)
            foreach (var o in _list)
                if (o is IReadOnlyable ro)
                    ro.SetReadOnly(onlyread, recursive);
    }
    private void CheckReadOnly()
    {
        if (IsReadOnly)
            throw new ReadOnlyException();
    }
    #endregion

    #region ISimplifyable
    public object ToSimpleObject()
    {
        var ret = new object?[_list.Count];

        for (int i = 0; i < _list.Count; i++)
        {
            var o = _list[i];

            if (o is ISimplifyable simplifyable)
                ret[i] = simplifyable.ToSimpleObject();
            else
                ret[i] = o;
        }

        return ret;
    }
    #endregion

    #region DynamicObject
    public override bool TryGetMember(
        GetMemberBinder binder, out object? result)
    {
        var match = IndexMemberRegex.Match(binder.Name);
        if (match.Success)
        {
            result = _list[int.Parse(match.Groups["index"].Value)];
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    public override bool TrySetMember(
        SetMemberBinder binder, object? value)
    {
        if (IsReadOnly)
            return false;

        var match = IndexMemberRegex.Match(binder.Name);
        if (match.Success)
        {
            _list[int.Parse(match.Groups["index"].Value)] = value.ToDynamic();
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region IList<object>

    public IEnumerator<object?> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(object? item)
    {
        CheckReadOnly();
        _list.Add(item.ToDynamic());
    }

    public void Clear()
    {
        CheckReadOnly();
        _list.Clear();
    }

    public bool Contains(object? item) => _list.Contains(item);

    public void CopyTo(object?[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public bool Remove(object? item)
    {
        CheckReadOnly();
        return _list.Remove(item);
    }

    public int Count => _list.Count;
    public bool IsReadOnly { get; private set; } = false;

    public int IndexOf(object? item) => _list.IndexOf(item);

    public void Insert(int index, object? item)
    {
        CheckReadOnly();
        _list.Insert(index, item.ToDynamic());
    }

    public void RemoveAt(int index)
    {
        CheckReadOnly();
        _list.RemoveAt(index);
    }

    public object? this[int index]
    {
        get => _list[index];
        set
        {
            CheckReadOnly();
            _list[index] = value.ToDynamic();
        }
    }
    #endregion

    #region Query
    public dynamic? Query(string query)
    {
        return (dynamic?)Query(new IQueryable.AccessorQuery(query));
    }

    public dynamic? Query(IEnumerable<string> query)
    {
        return (dynamic?)Query(new IQueryable.AccessorQuery(query));
    }

    protected object? Query(IQueryable.AccessorQuery query)
    {
        return query.Access(this);
    }

    public object? Access(IQueryable.IAccessor accessor)
    {
        if (accessor is IQueryable.IndexAccessor indexAccessor)
            return _list[indexAccessor.Index];

        throw new IQueryable.AccessorMismatch($"Can't use {accessor} on {this}");
    }
    #endregion

    public override string ToString() => this.ToDebugString();
}