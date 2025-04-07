namespace Maukka.Models
{
    public class TopsSize : ClothingSize
    {
        private float _heightCm;
        private float _chestCm;
        private float _sizeNbr;
        
        public float ChestCm
        {
            get => _chestCm;
            set => _chestCm = (float)Math.Round(value, 1);
        }
        
        public float HeightCm
        {
            get => _heightCm;
            set => _heightCm = (float)Math.Round(value, 1);
        }
        
        public float SizeNbr
        {
            get => _sizeNbr;
            set => _sizeNbr = (float)Math.Round(value, 1);
        }
        
        public string SizeCode { get; set; }

        public TopsSize() {}
        public TopsSize(CountryCode countryCode, float heightCm, float chestCm, 
               float sizeNbr, string sizeCode)
        {
            Category = ClothingCategory.Tops;
            CountryCode = countryCode;
            HeightCm = heightCm;
            ChestCm = chestCm;
            SizeNbr = sizeNbr;
            SizeCode = sizeCode;
        }
    }
}