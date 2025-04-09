using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities.Converters
{
    public class CountryCodeConverter : JsonConverter<CountryCode>
    {
        public override CountryCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            
            if (stringValue is null) return CountryCode.Unknown;
            
            if (Enum.TryParse(typeof(CountryCode), stringValue, true, out object result))
            {
                return (CountryCode)result;
            }
            
            // handle possible variations
            return stringValue.ToLowerInvariant() switch
            {
                _ => CountryCode.Unknown,
            };
        }

        public override void Write(Utf8JsonWriter writer, CountryCode value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString().ToLowerInvariant());
        
        public static CountryCode Parse(string category) 
            => Enum.TryParse(typeof(CountryCode), category, true, out object result) ?
                (CountryCode)result : CountryCode.Unknown;
    }
}