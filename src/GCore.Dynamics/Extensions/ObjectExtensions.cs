using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GCore.Dynamics.Traits;

namespace GCore.Dynamics.Extensions;

public static class ObjectExtensions
{

    private static readonly CultureInfo DefaultCultureInfo = new CultureInfo("en-US");

    public static bool IsNumericType(this object? self)
    {
        if (self is null) return false;
        switch (Type.GetTypeCode(self.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }

    public static dynamic? ToDynamic(this object? obj)
    {
        if(obj is null)
            return null;
        if(obj is IToDynamic tdy)
            return tdy.ToDynamic();
        if (obj.IsNumericType() ||
            obj is bool ||
            obj is string)
            return obj;
        if (obj is char c)
            return new string(new char[] { c });
        if (obj is DynamicDict || obj is DynamicList)
            return obj;
        if (obj is IDictionary dict)
            return DynamicDict.FromDict(dict);
        if (obj is IEnumerable enu)
            return DynamicList.FromEnumerable(enu);

        return DynamicDict.FromReflectedObject(obj);
    }

    private static Dictionary<object, object> ConvertToDictionary(System.Collections.IDictionary iDic)
    {
        var dic = new Dictionary<object, object>();
        var enumerator = iDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            dic[enumerator.Key] = enumerator.Value;
        }
        return dic;
    }

    private static object?[] ConvertToArray(IEnumerable enu)
    {
        var list = new List<object?>();
        foreach (var item in enu)
            list.Add(item);
        return list.ToArray();
    }

    internal static void ToDebugString(this object? obj, IndentedTextWriter writer)
    {
        if (obj is null)
            writer.Write("null");
        else if (obj.IsNumericType() ||
            obj is bool)
            writer.Write(Convert.ToString(obj, DefaultCultureInfo));
        else if (obj is char c)
            writer.Write("'" + c.ToLiteral() + "'");
        else if (obj is string s)
            writer.Write("\"" + s.ToLiteral() + "\"");
        else if (obj is IDictionary<string, object?> dict)
        {
            writer.WriteLine("{");
            writer.Indent++;
            int i = 0;
            foreach (var o in dict)
            {
                o.Key.ToDebugString(writer);
                writer.Write(":");
                o.Value.ToDebugString(writer);
                if(i < dict.Count - 1) writer.Write(",");
                writer.WriteLine();
                i++;
            }

            writer.Indent--;
            writer.Write("}");
        }
        else if (obj is IEnumerable<object?> enu)
        {
            writer.WriteLine("[");
            writer.Indent++;
            var a = ConvertToArray(enu);
            int i = 0;
            foreach (var o in a)
            {
                o.ToDebugString(writer);
                if (i < a.Length - 1) writer.Write(",");
                writer.WriteLine();
                i++;
            }

            writer.Indent--;
            writer.Write("]");
        }
        else
            writer.Write(obj);
    }

    internal static string ToDebugString(this object? o)
    {
        using (var output = new StringWriter())
        using (var writer = new IndentedTextWriter(output))
        {
            o.ToDebugString(writer);
            return output.ToString();
        }
    }
}