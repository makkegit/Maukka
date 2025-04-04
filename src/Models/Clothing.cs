using System.Text.Json.Serialization;

namespace Maukka.Models
{
    public abstract class Clothing
    {
        public ClothingId Id { get; set; }
        public abstract BrandClothingId BrandClothingId { get; set; }
        public abstract string ClothingName { get; set; }
        public string Alias { get; set; } = string.Empty;
        public abstract ClothingSize Size { get; set; }  
    }

}