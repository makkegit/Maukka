using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities.Converters
{
    public class BrandIdConverter : JsonConverter<BrandId>
    {
        public override BrandId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (BrandId)reader.GetInt32(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? (BrandId)value : throw new JsonException($"Cannot convert string value: {value} to {nameof(BrandId)}"),
                _ => throw new JsonException($"Cannot convert to {nameof(BrandId)} from type: {reader.TokenType}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, BrandId value, JsonSerializerOptions options)
        {
            // You can choose to write just the integer value
            writer.WriteNumberValue(value.Value);
        
            // Or write as an object with Value property
            // writer.WriteStartObject();
            // writer.WriteNumber("Value", value.Value);
            // writer.WriteEndObject();
        }
        
    }
}