﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GCore.Dynamics.Json;

public static class DynamicJson
{
    public static dynamic? FromJsonElement(JsonElement elem)
    {
        switch (elem.ValueKind)
        {
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Number:
                return elem.GetDouble();
            case JsonValueKind.False:
                return false;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                return elem.GetString();
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var item in elem.EnumerateObject())
                    dict.Add(item.Name, FromJsonElement(item.Value));
                return new DynamicDict(dict);
                //return new DynamicDict(
                //    elem
                //        .EnumerateObject()
                //        .ToDictionary(o => o.Name, o => FromJsonElement(o.Value))
                //    );
            case JsonValueKind.Array:
                return new DynamicList(elem.EnumerateArray().Select(o => FromJsonElement(o)));
        }

        return null;
    }

    public static dynamic? Deserialize(string json)
    {
        return FromJsonElement(JsonDocument.Parse(json).RootElement);
    }

    public static string Serialise(object? obj, JsonSerializerOptions? options = null)
    {
        if (obj is null)
            return "null";

        if (obj is ISimplifyable s)
            obj = s.ToSimpleObject();

        return JsonSerializer.Serialize(obj, options);
    }

    public static dynamic? QueryFile(string file, string query)
    {
        return Deserialize(File.ReadAllText(file))?.Query(query);
    }

    public static dynamic? QueryFile(string fileQuery)
    {
        var split = fileQuery.Split('?');

        var json = Deserialize(File.ReadAllText(split[0]));

        if (split.Length == 1)
            return json;

        return json?.Query(string.Join("?", split.Skip(1)));
    }
}