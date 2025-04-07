using System.Text.Json.Serialization;

namespace Maukka.Models
{
    public abstract class Clothing
    {
        public ClothingId Id { get; set; }
        public abstract BrandClothingId BrandClothingId { get; set; }
        public virtual ClothingCategory ClothingCategory => ClothingCategory.NotSet;
        public abstract string ClothingName { get; set; }
        public string Alias { get; set; } = string.Empty;
        public ClothingSize? Size { get; set; }  
    }

}