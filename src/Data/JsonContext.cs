using System.Text.Json.Serialization;
using Maukka.Models;

[JsonSerializable(typeof(Wardrobe))]
[JsonSerializable(typeof(Clothing))]
[JsonSerializable(typeof(Brand))]
[JsonSerializable(typeof(BrandsJson))]
[JsonSerializable(typeof(ClothingSizesJSON))]
[JsonSerializable(typeof(BrandClothingJson))]
[JsonSerializable(typeof(ClothingJson))]
[JsonSerializable(typeof(WardrobesJson))]
public partial class JsonContext : JsonSerializerContext
{
}