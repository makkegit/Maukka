namespace Maukka.Models
{
    public abstract class Bottoms : Clothing
    {
        public override ClothingCategory ClothingCategory => ClothingCategory.Bottoms;
        public new BottomsSize? Size { get; set; }
    }

}