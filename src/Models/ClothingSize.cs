using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class ClothingSize
    {
        public const string HeightKey = "height";
        public const string ChestKey = "chest";
        public const string SizeNbrKey = "sizeNbr";
        public const string WaistKey = "waist";
        public const string HipsKey = "hips";
        public const string InsideLegLengthKey = "insideLegLength";
        
        public int SizeId { get; set; }
        
        [JsonConverter(typeof(BrandIdConverter))]
        public BrandId BrandId { get; set; }
        
        [JsonConverter(typeof(CountryCodeConverter))]
        public CountryCode CountryCode { get; set; }
        
        [JsonConverter(typeof(MeasurementUnitConverter))]
        public MeasurementUnit MeasurementUnit { get; set; }
        
        [JsonConverter(typeof(ClothingCategoryConverter))]
        public ClothingCategory Category { get; set; }
        public string SizeCode { get; set; }
        public int AgeFromMonths { get; set; }
        public int AgeToMonths { get; set; }
        
        public float AgeFromYears { get; set; }
        public float AgeToYears { get; set; }
        
        public Dictionary<string, float> Measurements { get; set; } 
            = new Dictionary<string, float>();

        public static ClothingSize CreateTopsSize(CountryCode countryCode,
            MeasurementUnit measurementUnit, string sizeCode, int ageFromMonths, int ageToMonths,
            float height, float chest, float sizeNbr)
        {
            var clothing = new ClothingSize()
            {
                CountryCode = countryCode,
                MeasurementUnit = measurementUnit,
                Category = ClothingCategory.Tops,
                SizeCode = sizeCode,
                AgeFromMonths = ageFromMonths,
                AgeToMonths = ageToMonths,
                AgeFromYears = (float)Math.Round(ageFromMonths / 12f * 2f, 1) / 2f,
                AgeToYears = (float)Math.Round(ageToMonths / 12f * 2f, 1) / 2f
            };
            
            clothing.Measurements.Add(ChestKey, chest);
            clothing.Measurements.Add(HeightKey, height);
            clothing.Measurements.Add(SizeNbrKey, sizeNbr);
            
            return clothing;
        }
        
        public static ClothingSize CreateBottomsSize(CountryCode countryCode,
            MeasurementUnit measurementUnit, string sizeCode, int ageFromMonths, int ageToMonths,
            float waist, float hips, float insideLegLength)
        {
            var clothing = new ClothingSize()
            {
                CountryCode = countryCode,
                MeasurementUnit = measurementUnit,
                Category = ClothingCategory.Bottoms,
                SizeCode = sizeCode,
                AgeFromMonths = ageFromMonths,
                AgeToMonths = ageToMonths,
                AgeFromYears = (float)Math.Round(ageFromMonths / 12f * 2f, 1) / 2f,
                AgeToYears = (float)Math.Round(ageToMonths / 12f * 2f, 1) / 2f
            };
            
            clothing.Measurements.Add(WaistKey, waist);
            clothing.Measurements.Add(HipsKey, hips);
            clothing.Measurements.Add(InsideLegLengthKey, insideLegLength);
            
            return clothing;
        }
    }

    public class ClothingSizesJSON
    {
        public List<ClothingSize> ClothingSizes { get; set; } = [];
    }
}