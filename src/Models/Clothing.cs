using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Clothing
    {
        [JsonConverter(typeof(ClothingIdConverter))]
        public ClothingId ClothingId { get; set; }
        
        [JsonConverter(typeof(BrandClothingIdConverter))]
        public BrandClothingId BrandClothingId { get; set; }
        public string BrandName { get; set; } 
        public string ClothingName { get; set; }
        [JsonConverter(typeof(ClothingCategoryConverter))]
        public ClothingCategory Category { get; set; }
        public ClothingSize Size { get; set; }
        public string Alias { get; set; }
        public int Quantity { get; set; }

        public static Clothing InitClothing(ClothingId id, BrandClothing brandClothing, ClothingSize clothingSize, int quantity = 1, string? alias = null)
        {
            return new Clothing
            {
                ClothingId = id,
                BrandClothingId = brandClothing.BrandClothingId,
                BrandName = brandClothing.Brand.BrandName,
                ClothingName = brandClothing.Name,
                Category = brandClothing.Category,
                Size = clothingSize,
                Quantity = quantity > 0 ? quantity : 1,
                Alias = alias ?? string.Empty,
            };
        }
    }
}