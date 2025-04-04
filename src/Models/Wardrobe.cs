using System.Text.Json.Serialization;

namespace Maukka.Models
{
    public class Wardrobe
    {
        public WardrobeId Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Clothing> Clothes { get; set; } = [];
    }

    public class WardrobesJson
    {
        public List<Wardrobe> Wardrobes { get; set; } = [];
    }
}