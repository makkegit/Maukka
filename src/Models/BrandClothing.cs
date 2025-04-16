using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class BrandClothing
    {
        [JsonConverter(typeof(BrandClothingIdConverter))]
        public BrandClothingId BrandClothingId { get; set; }
        public Brand Brand  { get; set; }
        public string Name { get; set; }
       
        [JsonConverter(typeof(ClothingCategoryConverter))]
        public ClothingCategory Category { get; set; }
        public List<ClothingSize> ClothingSizes { get; set; } = [];
    }
}