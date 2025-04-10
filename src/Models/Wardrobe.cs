using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Wardrobe
    {
        [JsonConverter(typeof(WardrobeIdConverter))]
        public WardrobeId WardrobeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Clothing> Items { get; set; } = [];
    }

}