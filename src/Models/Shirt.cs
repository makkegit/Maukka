namespace Maukka.Models
{
    public class Shirt : Tops
    {
        public override BrandClothingId BrandClothingId { get; set; }
        public override string ClothingName { get; set; }
    }
}