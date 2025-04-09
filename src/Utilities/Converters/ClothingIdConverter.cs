using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities.Converters
{
    public class ClothingIdConverter  : JsonConverter<ClothingId>
    {
        public override ClothingId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? 
                    (ClothingId)value : throw new JsonException($"Cannot convert string value: {value} to {nameof(ClothingId)}"),
                _ => throw new JsonException($"Cannot convert to {nameof(ClothingId)} from type: {reader.TokenType}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, ClothingId value, JsonSerializerOptions options)
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