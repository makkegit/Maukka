namespace Maukka.Models
{
    public class Pants : Bottoms
    {
        public override BrandClothingId BrandClothingId { get; set; }
        public override string ClothingName { get; set; }
    }
}