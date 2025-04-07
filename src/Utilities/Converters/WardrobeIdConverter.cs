using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities
{
    public class WardrobeIdConverter : JsonConverter<WardrobeId>
    {
        public override WardrobeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (WardrobeId)reader.GetInt32(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? (WardrobeId)value : throw new JsonException($"Cannot convert string value: {value} to {nameof(WardrobeId)}"),
                _ => throw new JsonException($"Cannot convert to {nameof(WardrobeId)} from type: {reader.TokenType}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, WardrobeId value, JsonSerializerOptions options)
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