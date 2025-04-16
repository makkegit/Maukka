using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Data
{
    [JsonSerializable(typeof(Brand))]
    [JsonSerializable(typeof(BrandClothing))]
    [JsonSerializable(typeof(Clothing))]
    [JsonSerializable(typeof(ClothingSize))]
    [JsonSerializable(typeof(Wardrobe))]
    [JsonSerializable(typeof(BrandsJson))]
    [JsonSerializable(typeof(BrandClothingJson))]
    [JsonSerializable(typeof(ClothingSizesJson))]
    [JsonSerializable(typeof(ClothingJson))]
    [JsonSerializable(typeof(WardrobesJson))]
    public partial class JsonContext : JsonSerializerContext
    {
    }
}