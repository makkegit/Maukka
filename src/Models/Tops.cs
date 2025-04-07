namespace Maukka.Models
{
    public abstract class Tops : Clothing
    {
        public override ClothingCategory ClothingCategory => ClothingCategory.Tops;
        public new TopsSize? Size { get; set; }
    }

}