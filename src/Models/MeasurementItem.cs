namespace Maukka.Models
{
    public class MeasurementItem
    {
        public MeasurementItem(string key, float value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public float Value { get; set; }
    }
}