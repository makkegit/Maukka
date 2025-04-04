namespace Maukka.Models
{
    public readonly record struct BrandClothingId
    {
        public int Value { get; }
        private BrandClothingId(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            Value = value;
        }
        
        public static implicit operator BrandClothingId(int value) => new BrandClothingId(value);
    }
}