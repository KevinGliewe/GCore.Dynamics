using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;
using GCore.Dynamics.Extensions;
using GCore.Dynamics.Traits;

namespace GCore.Dynamics;

public class DynamicDict : DynamicObject, IDictionary<string, object?>, IReadOnlyable, ISimplifyable, IQueryable, IQueryable.IAccessible
{
    protected IDictionary<string, object?> _dict = new Dictionary<string, object?>();

    public static DynamicDict FromDict(IDictionary dict)
    {
        var newDict = new Dictionary<string, object?>();
        foreach (DictionaryEntry entry in dict)
        {
            newDict[entry.Key.ToString()] = entry.Value.ToDynamic();
        }
        return new DynamicDict(newDict);
    }

    public static DynamicDict FromReflectedObject(object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        var newDict = new Dictionary<string, object?>();
        foreach (var propertyInfo in obj.GetType().GetProperties())
        {
            newDict[propertyInfo.Name] = propertyInfo.GetValue(obj).ToDynamic();
        }
        return new DynamicDict(newDict);
    }

    public DynamicDict() {}

    public DynamicDict(IDictionary<string, object?> dict)
    {
        _dict = dict;
    }

    #region IReadOnlyable
    public void SetReadOnly(bool onlyread, bool recursive = true)
    {
        IsReadOnly = onlyread;

        if(recursive)
            foreach (var o in _dict)
                if(o.Value is IReadOnlyable ro)
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
        var dict = new Dictionary<string, object?>();

        foreach (var o in _dict)
        {
            if (o.Value is ISimplifyable simplifyable)
                dict[o.Key] = simplifyable.ToSimpleObject();
            else
                dict[o.Key] = o.Value;
        }

        return dict;
    }
    #endregion

    #region DynamicObject

    public override bool TryGetMember(
        GetMemberBinder binder, out object? result)
    {
        string name = binder.Name;

        return _dict.TryGetValue(name, out result);
    }

    public override bool TrySetMember(
        SetMemberBinder binder, object value)
    {
        if (IsReadOnly)
            return false;

        _dict[binder.Name] = value.ToDynamic();
        return true;
    }

    #endregion

    #region IDictionary<string, object?>

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<string, object?> item)
    {
        CheckReadOnly();
        _dict.Add(item.Key, item.Value.ToDynamic());
    }

    public void Clear()
    {
        CheckReadOnly();
        _dict.Clear();
    }

    public bool Contains(KeyValuePair<string, object?> item) => _dict.Contains(item);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => throw new NotSupportedException();

    public bool Remove(KeyValuePair<string, object?> item)
    {
        CheckReadOnly();
        return _dict.Remove(item.Key);
    }

    public int Count => _dict.Count;
    public bool IsReadOnly { get; private set; } = false;

    public void Add(string key, object? value)
    {
        CheckReadOnly();
        _dict.Add(key, value.ToDynamic());
    }

    public bool ContainsKey(string key) => _dict.ContainsKey(key);

    public bool Remove(string key)
    {
        CheckReadOnly();
        return _dict.Remove(key);
    }

    public bool TryGetValue(string key, out object? value) => _dict.TryGetValue(key, out value);

    public object? this[string key]
    {
        get => _dict[key];
        set
        {
            CheckReadOnly();
            _dict[key] = value.ToDynamic();
        }
    }

    public ICollection<string> Keys => _dict.Keys;
    public ICollection<object?> Values => _dict.Values;

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

    public dynamic? QueryNull(string query)
    {
        try
        {
            return Query(query);
        }
        catch
        {
            return null;
        }
    }

    public dynamic? QueryNull(IEnumerable<string> query)
    {
        try
        {
            return Query(query);
        }
        catch
        {
            return null;
        }
    }

    protected object? Query(IQueryable.AccessorQuery query)
    {
        return query.Access(this);
    }

    public object? Access(IQueryable.IAccessor accessor)
    {
        if (accessor is IQueryable.MemberAccessor memberAccessor)
            return _dict[memberAccessor.Name];

        throw new IQueryable.AccessorMismatch($"Can't use {accessor} on {this}");
    }
    #endregion

    public override string ToString() => this.ToDebugString();
}