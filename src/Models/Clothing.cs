using System.Text.Json.Serialization;

namespace Maukka.Models
{
    [JsonConverter(typeof(ClothingConverter))]
    public abstract class Clothing
    {
        //public ClothingId ClothingId { get; set; }
        public abstract BrandClothingId BrandClothingId { get; set; }
        
        [JsonConverter(typeof(ClothingCategoryConverter))]
        public virtual ClothingCategory ClothingCategory => ClothingCategory.NotSet;
        public abstract string ClothingName { get; set; }
        public string Alias { get; set; } = string.Empty;
        public ClothingSize? Size { get; set; }  
    }

}