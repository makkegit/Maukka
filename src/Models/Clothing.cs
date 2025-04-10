using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Clothing
    {
        [JsonConverter(typeof(BrandClothingIdConverter))]
        public BrandClothingId ClothingId { get; set; }
        public string BrandName { get; set; }
        public string ClothingName { get; set; }
        [JsonConverter(typeof(ClothingCategoryConverter))]
        public ClothingCategory Category { get; set; }
        public ClothingSize Size { get; set; }
        public string Alias { get; set; }

        public static Clothing InitClothing(BrandClothing brandClothing, ClothingSize clothingSize, string? alias = null)
        {
            return new Clothing
            {
                ClothingId = brandClothing.BrandClothingId,
                BrandName = brandClothing.Brand.BrandName,
                ClothingName = brandClothing.Name,
                Category = brandClothing.Category,
                Size = clothingSize,
                Alias = alias ?? string.Empty,
            };
        }
    }
}