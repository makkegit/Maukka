using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities.Converters
{
    public class MeasurementUnitConverter : JsonConverter<MeasurementUnit>
    {
        public override MeasurementUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            
            if (stringValue is null) return MeasurementUnit.Centimeter;
            
            if (Enum.TryParse(typeof(MeasurementUnit), stringValue, true, out object result))
            {
                return (MeasurementUnit)result;
            }
            
            // handle possible variations
            return stringValue.ToLowerInvariant() switch
            {
                "cm" => MeasurementUnit.Centimeter,
                "mm" => MeasurementUnit.Millimeter,
                "in" => MeasurementUnit.Inch,
                _ => MeasurementUnit.Centimeter,
            };
        }

        public override void Write(Utf8JsonWriter writer, MeasurementUnit value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString().ToLowerInvariant());
        
        public static MeasurementUnit Parse(string category) 
            => Enum.TryParse(typeof(MeasurementUnit), category, true, out object result) ?
                (MeasurementUnit)result : MeasurementUnit.Centimeter;
    }
}