namespace Maukka.Models
{
    public abstract class ClothingSize
    {
        public CountryCode CountryCode { get; set; }
        public ClothingCategory Category { get; set; }
        
        public int AgeFromMonths { get; set; }
        public int AgeToMonths { get; set; }
        
        public float AgeFromYears { get; set; }
        public float AgeToYears { get; set; }
    }
}