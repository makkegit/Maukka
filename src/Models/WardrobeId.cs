namespace Maukka.Models
{
    public readonly record struct WardrobeId
    {
        public int Value { get; init; }
        private WardrobeId(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            Value = value;
        }

        public static implicit operator WardrobeId(int value) => new WardrobeId(value);
    }
}