namespace Maukka.Models
{
    public class BrandClothing
    {
        public BrandClothingId BrandClothingId { get; set; }
        public Brand Brand  { get; set; }
        public string Name { get; set; }
        public ClothingCategory Category { get; set; }
        public IList<ClothingSize> ClothingSizes { get; set; }
    }

    public class BrandClothingJson
    {
        public List<BrandClothing> BrandClothing { get; set; } = [];
    }
}