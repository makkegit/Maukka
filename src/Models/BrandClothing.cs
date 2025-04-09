namespace Maukka.Models
{
    public class BrandClothing
    {
        public BrandClothingId Id { get; set; }
        public Brand Brand  { get; set; }
        public string Name { get; set; }
        public ClothingCategory Category { get; set; }
        public IList<ClothingSize> ClothingSizes { get; set; }
    }

}