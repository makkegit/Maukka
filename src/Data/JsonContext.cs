using System.Text.Json.Serialization;
using Maukka.Models;

[JsonSerializable(typeof(Wardrobe))]
[JsonSerializable(typeof(Clothing))]
[JsonSerializable(typeof(WardrobesJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext
{
}