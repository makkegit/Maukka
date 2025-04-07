namespace Maukka.Models
{
    public class BottomsSize : ClothingSize
    {
        private float _waistCm;
        private float _hipsCm;
        private float _insideLegLengthCm;
        
        public float WaistCm { get => _waistCm; set => _waistCm = (float)Math.Round(value, 1); }
        public float HipsCm { get => _hipsCm; set => _hipsCm = (float)Math.Round(value, 1); }
        public float InsideLegLengthCm { get => _insideLegLengthCm; set => _insideLegLengthCm = (float)Math.Round(value, 1); }
        public string SizeCode { get; set; }

        public BottomsSize(CountryCode countryCode, 
            float waistCm, float hipsCm, float insideLegLengthCm,
            string sizeCode)
        {
            Category = ClothingCategory.Bottoms;
            CountryCode = countryCode;
            WaistCm = waistCm;
            HipsCm = hipsCm;
            InsideLegLengthCm = insideLegLengthCm;
            SizeCode = sizeCode;
        }
    }
}