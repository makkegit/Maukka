using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities
{
    public class ClothingConverter : JsonConverter<Clothing>
    {
        public override Clothing Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            // Clone the reader to inspect the JSON without advancing the original
            var readerClone = reader;
            using (JsonDocument doc = JsonDocument.ParseValue(ref readerClone))
            {
                var root = doc.RootElement;
            
                // Extract the ClothingCategory value
                string categoryStr = root.GetProperty(nameof(ClothingCategory)).GetString()!;
                var category = ClothingCategoryConverter.Parse(categoryStr);

                // Determine the concrete type
                Type targetType = category switch
                {
                    ClothingCategory.Tops => typeof(Shirt),
                    ClothingCategory.Bottoms => typeof(Pants), // Add your bottoms class
                    _ => throw new JsonException($"Unknown clothing category: {categoryStr}")
                };

                // Deserialize using a new options instance to avoid infinite recursion
                var newOptions = new JsonSerializerOptions()
                {
                    Converters = { new BrandClothingIdConverter() },
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };
                
                return (Clothing)JsonSerializer.Deserialize(ref reader, targetType, newOptions)!;
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            Clothing value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}