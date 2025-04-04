namespace Maukka.Models
{
    public readonly record struct BrandId
    {
        public int Value { get; init; }
        public BrandId(int value) 
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            Value = value;
        }
        
        public static implicit operator BrandId (int value) => new(value);
    }
}