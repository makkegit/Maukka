using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities
{
    public class BrandClothingIdConverter : JsonConverter<BrandClothingId>
    {
        public override BrandClothingId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (BrandClothingId)reader.GetInt32(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? (BrandClothingId)value : throw new JsonException($"Cannot convert string value: {value} to {nameof(BrandClothingId)}"),
                _ => throw new JsonException($"Cannot convert to {nameof(BrandClothingId)} from type: {reader.TokenType}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, BrandClothingId value, JsonSerializerOptions options)
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