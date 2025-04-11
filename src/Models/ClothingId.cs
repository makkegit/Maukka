namespace Maukka.Models
{
    public readonly record struct ClothingId
    {
        public int Value { get; init; }

        private ClothingId(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            Value = value;
        }
        
        public static implicit operator ClothingId (int value) => new ClothingId(value);
    }
}