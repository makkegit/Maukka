using System.Text.Json.Serialization;
using Maukka.Models;

[JsonSerializable(typeof(Wardrobe))]
[JsonSerializable(typeof(Clothing))]
[JsonSerializable(typeof(BrandsJSON))]
[JsonSerializable(typeof(ClothingSizesJSON))]
[JsonSerializable(typeof(BrandClothingJson))]
[JsonSerializable(typeof(ClothingJson))]
[JsonSerializable(typeof(WardrobesJson))]
public partial class JsonContext : JsonSerializerContext
{
}