namespace Maukka.Models
{
    public readonly record struct BrandClothingId
    {
        public int Value { get; }
        private BrandClothingId(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(BrandClothingId));
            Value = value;
        }
        
        public static implicit operator BrandClothingId(int value) => new BrandClothingId(value);
    }
}