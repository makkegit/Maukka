using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Wardrobe
    {
        [JsonConverter(typeof(WardrobeIdConverter))]
        public WardrobeId WardrobeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Clothing> Items { get; set; } = [];
        public WardrobeStatistics Statistics { get; set; }
        
        public static Wardrobe InitWardrobe(WardrobeId id, string name, string description, IEnumerable<Clothing> clothing)
        {
            return new Wardrobe
            {
                WardrobeId = id,
                Name = name,
                Description = description,
                Items = clothing.ToList(),
                Statistics = WardrobeStatistics.CreateFromClothing(clothing)
            };       
        }
        
        public void AddClothing(Clothing clothing)
        {
            Items.Add(clothing);
            Statistics = WardrobeStatistics.CreateFromClothing(Items);
        }
    }

    public class WardrobeStatistics
    {
        public int TotalTops { get; set; }
        public int TotalBottoms { get; set; }
        public int TotalShoes { get; set; }
        public int TotalAccessories { get; set; }

        public int TotalClothing => TotalTops + TotalBottoms + TotalShoes + TotalAccessories;

        public static WardrobeStatistics CreateFromClothing(IEnumerable<Clothing> clothing)
        {
            return new WardrobeStatistics
            {
                TotalTops = clothing.Count(c => c.Category == ClothingCategory.Tops),
                TotalBottoms = clothing.Count(c => c.Category == ClothingCategory.Bottoms),
                TotalShoes = clothing.Count(c => c.Category == ClothingCategory.Shoes),
                TotalAccessories = clothing.Count(c => c.Category == ClothingCategory.Hats)
            };
        }
    }
}