using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSSourceGenerator
{
    public readonly record struct JsonNode
    {
        public readonly string Name;
        public readonly Type Type;

        public JsonNode(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public static class JsonParser
    {
        public static Type ToArrayType(this JsonValueKind jsonValueKind)
        {
            return jsonValueKind switch
            {
                JsonValueKind.String => typeof(string[]),
                JsonValueKind.Number => typeof(int[]),
                JsonValueKind.True or JsonValueKind.False => typeof(bool[]),
                _ => typeof(object[]),
            };
        }

        public static IEnumerable<SectionToGenerate> Traverse(JsonElement element)
        {
            foreach (var item in element.EnumerateObject())
            {
                var properties = TraverseObject(item.Value, string.Empty).Select(x => new PropertyToGenerate(x.Name, x.Type)).ToImmutableArray();
                yield return new SectionToGenerate(item.Name, properties);
            }
        }

        private static IEnumerable<JsonNode> TraverseObject(JsonElement element, string path)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string newPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}_{property.Name}";
                        foreach (var item in TraverseObject(property.Value, newPath))
                        {
                            yield return item;
                        }
                    }
                    break;

                case JsonValueKind.Array:
                    var elementType = element.EnumerateArray().FirstOrDefault().ValueKind.ToArrayType();
                    yield return new JsonNode(path, elementType);
                    break;
                case JsonValueKind.String:
                    yield return new JsonNode(path, typeof(string));
                    break;

                case JsonValueKind.Number:
                    yield return new JsonNode(path, typeof(int));
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    yield return new JsonNode(path, typeof(bool));
                    break;

                case JsonValueKind.Null:
                    yield return new JsonNode(path, typeof(object));
                    break;
            }
        }
    }

}
