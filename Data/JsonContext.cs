using System.Text.Json.Serialization;
using Maukka.Models;

[JsonSerializable(typeof(Wardrobe))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext
{
}