using System.Text.Json.Serialization;
using Maukka.Models;

[JsonSerializable(typeof(Wardrobe))]
[JsonSerializable(typeof(Clothing))]
[JsonSerializable(typeof(WardrobesJson))]
public partial class JsonContext : JsonSerializerContext
{
}