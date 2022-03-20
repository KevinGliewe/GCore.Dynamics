using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GCore.Json
{
    // https://stackoverflow.com/a/58679930
    public class JsonDynamic : DynamicObject
    {
        public static readonly Regex ArrayQueryRegex = new Regex(@"^(?<name>.*)\[(?<index>\d+)\]$");

        public JsonElement RealObject { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object? result) {
            // Get the property value
            var srcData = RealObject.GetProperty(binder.Name);

            result = ToDynamic(srcData);

            // Always return true; other exceptions may have already been thrown if needed
            return true;
        }

        public dynamic? this[int index]
        {
            get {
                if(RealObject.ValueKind == JsonValueKind.Array)
                    return ToDynamic(RealObject[index]);

                string sindex = index.ToString();
                if(RealObject.ValueKind == JsonValueKind.Object)
                    return ToDynamic(RealObject.EnumerateObject().FirstOrDefault(x => x.Name == sindex).Value);

                return null;
            }
        }

        public dynamic? this[string index]
        {
            get {
                return Query(index);
            }
        }

        public dynamic? Query(IEnumerable<string> query)
        {
            JsonElement elem = RealObject;

            var queryHistory = new List<string>();

            foreach (var q in query)
            {
                if(q.Trim() == "")
                    continue;

                if(elem.ValueKind != JsonValueKind.Object)
                    return null; //throw new Exception(string.Join(".", queryHistory) + " is not a object!");


                var arrMatch = ArrayQueryRegex.Match(q);

                var elemName = arrMatch.Success ? arrMatch.Groups["name"].Value : q;

                queryHistory.Add(elemName);

                if(elem.EnumerateObject().Count(p => p.Name == elemName) == 0)
                    return null; //throw new Exception(string.Join(".", queryHistory) + $" has no property '{elemName}'!");

                elem = elem.GetProperty(elemName);

                // Is array query
                if (arrMatch.Success)
                {
                    var index = Int32.Parse(arrMatch.Groups["index"].Value);

                    if(elem.ValueKind != JsonValueKind.Array)
                        return null; //throw new Exception(string.Join(".", queryHistory) + " is not a array!");

                    queryHistory.Add(index.ToString());

                    elem = elem.EnumerateArray().ToArray()[index];
                }
            }

            return ToDynamic(elem);
        }

        public dynamic? Query(string query)
        {
            return Query(query.Split('.'));
        }

        public IEnumerable<dynamic?> Enumerate()
        {
            if(RealObject.ValueKind == JsonValueKind.Array)
            {
                foreach(var elem in RealObject.EnumerateArray())
                {
                    yield return ToDynamic(elem);
                }
            }
            else if(RealObject.ValueKind == JsonValueKind.Object)
            {
                foreach(var elem in RealObject.EnumerateObject())
                {
                    yield return new KeyValuePair<string, dynamic?>(elem.Name,ToDynamic(elem.Value));
                }
            }
        }

        public static dynamic? ToDynamic(JsonElement elem)
        {
            switch (elem.ValueKind) {
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
                    return new JsonDynamic {
                        RealObject = elem
                    };
                case JsonValueKind.Array:
                    return elem.EnumerateArray()
                        .Select(o => new JsonDynamic { RealObject = o })
                        .ToArray();
            }

            return null;
        }

        public static JsonDynamic Parse(string json) {
            var result = JsonDocument.Parse(json);
            return new JsonDynamic {
                RealObject = result.RootElement
            };
        }

        public static dynamic? QueryFile(string file, string query)
        {
            return Parse(File.ReadAllText(file)).Query(query);
        }

        public static dynamic? QueryFile(string fileQuery)
        {
            var split = fileQuery.Split('?');

            var json = Parse(File.ReadAllText(split[0]));

            if (split.Length == 1)
                return json;

            return json.Query(string.Join("?", split.Skip(1)));
        }

        public override string ToString()
        {
            return RealObject.ToString();
        }
    }
}