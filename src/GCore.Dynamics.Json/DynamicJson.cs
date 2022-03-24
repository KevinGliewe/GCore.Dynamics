using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GCore.Dynamics.Traits;

namespace GCore.Dynamics.Json;

public static class DynamicJson
{
    public static JsonDocumentOptions DefaultJsonDocumentOptions { get; set; } = new JsonDocumentOptions()
    {
        // Ignore comments inside json so the parser doesn't crash
        CommentHandling = JsonCommentHandling.Skip,
    };

    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new JsonSerializerOptions()
    {
        // Make it readable by default
        WriteIndented = true
    };

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
            case JsonValueKind.Array:
                return new DynamicList(elem.EnumerateArray().Select(o => FromJsonElement(o)));
        }

        return null;
    }

    public static dynamic? Deserialize(string json, JsonDocumentOptions? options = null)
    {
        return FromJsonElement(JsonDocument.Parse(json, options ?? DefaultJsonDocumentOptions).RootElement);
    }

    public static string Serialise(object? obj, JsonSerializerOptions? options = null)
    {
        if (obj is null)
            return "null";

        if (obj is ISimplifyable s)
            obj = s.ToSimpleObject();

        return JsonSerializer.Serialize(obj, options ?? DefaultJsonSerializerOptions);
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

    public static dynamic? QueryNullFile(string file, string query)
    {
        try
        {
            return QueryFile(file, query);
        }
        catch
        {
            return null;
        }
    }

    public static dynamic? QueryNullFile(string fileQuery)
    {
        try
        {
            return QueryFile(fileQuery);
        }
        catch
        {
            return null;
        }
    }

    public static void WriteFile(string file, dynamic? obj, JsonSerializerOptions? options = null) =>
        File.WriteAllText(file, Serialise(obj, options));
}