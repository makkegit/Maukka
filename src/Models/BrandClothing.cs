namespace Maukka.Models
{
    public class BrandClothing
    {
        public BrandClothingId Id { get; set; }
        public int BrandId { get; set; }
        public string ClothingName { get; set; }
        public ClotingCategory Category { get; set; }
        public IList<ClothingSize> ClothingSizes { get; set; }
    }

}