namespace Maukka.Models
{
    public readonly record struct ClothingId
    {
        public int Value { get; init; }

        private ClothingId(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            Value = value;
        }
        
        public static implicit operator ClothingId (int value) => new ClothingId(value);
    }
}