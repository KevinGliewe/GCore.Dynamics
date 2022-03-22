using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace GCore.Dynamics;

public interface IQueryable
{
    dynamic? Query(string query);
    dynamic? Query(IEnumerable<string> query);

    dynamic? QueryNull(string query);
    dynamic? QueryNull(IEnumerable<string> query);

    public interface IAccessor
    {
        object? Access(object obj);
    }

    public interface IAccessible
    {
        object? Access(IAccessor accessor);
    }

    public class AccessorQuery : IAccessor
    {
        public static readonly Regex ArrayQueryRegex = new Regex(@"^(?<name>[^\[]+)?(\[(?<index>\d+)\])*$");

        private IAccessor[] _query;
        public ReadOnlyCollection<IAccessor> Query => Array.AsReadOnly(_query);

        private int _accessorIndex = 0;

        public AccessorQuery(string query) : this(query.Split('.')) { }

        public AccessorQuery(IEnumerable<string> query)
        {
            var tmp = new List<IAccessor>();

            foreach (var q in query)
            {
                var match = ArrayQueryRegex.Match(q);
                if (!match.Success)
                    throw new ArgumentException($"Can't parse query element '{Regex.Unescape(q)}'");

                var gname = match.Groups["name"];
                var gindex = match.Groups["index"];

                if (gname.Success)
                    tmp.Add(new MemberAccessor(gname.Value));

                if (gindex.Success)
                {
                    foreach (Capture gindexCapture in gindex.Captures)
                    {
                        tmp.Add(new IndexAccessor(int.Parse(gindexCapture.Value)));
                    }
                }
            }

            _query = tmp.ToArray();
        }

        public bool IsEndOfQuerry() => _accessorIndex >= _query.Length;
        public void Reset() => _accessorIndex = 0;

        public IAccessor NextAccessor()
        {
            _accessorIndex++;
            return _query[_accessorIndex-1];
        }

        public object? Access(object obj)
        {
            object? o = obj;
            while (!IsEndOfQuerry())
            {
                if (o is null)
                    throw new AccessorMismatch("Can't access null");
                o = NextAccessor().Access(o);
            }

            return o;
        }

        public object? AccessNull(object obj)
        {
            try
            {
                return Access(obj);
            }
            catch
            {
                return null;
            }
        }
    }

    public record MemberAccessor : IAccessor
    {
        public string Name { get; protected set; }

        public MemberAccessor(string name)
        {
            Name = name;
        }

        public object? Access(object? obj)
        {
            if(obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is IAccessible acc)
                return acc.Access(this);

            if (obj is IDictionary dict)
                if (dict.Contains(Name))
                    return dict[Name];

            var prop = obj.GetType().GetProperty(Name);
            if(prop != null)
                return prop.GetValue(obj, null);

            throw new AccessorMismatch($"Can't access '{Name}' from {obj}");
        }
    }

    public class IndexAccessor : IAccessor
    {
        public int Index { get; protected set; }

        public IndexAccessor(int index)
        {
            Index = index;
        }

        public object? Access(object? obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is IAccessible acc)
                return acc.Access(this);

            if (obj is IEnumerable enu)
            {
                int idx = 0;
                foreach (var item in enu)
                {
                    if (idx == Index)
                        return item;
                    idx++;
                }

                throw new IndexOutOfRangeException();
            }
            else
            {
                throw new AccessorMismatch($"Can't access [{Index}] from {obj}");
            }
        }
    }

    public class AccessorMismatch : Exception
    {
        public AccessorMismatch(string text) : base(text) { }
    }
}