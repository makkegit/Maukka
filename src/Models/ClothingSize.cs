namespace Maukka.Models
{
    public abstract class ClothingSize
    {
        public CountryCode CountryCode { get; set; }
        public abstract ClotingCategory Category { get; set; }
    }
}