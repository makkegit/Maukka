using System.Text.Json;
using System.Text.Json.Serialization;
using Maukka.Models;

namespace Maukka.Utilities.Converters
{
    public class ClothingCategoryConverter : JsonConverter<ClothingCategory>
    {
        public override ClothingCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            
            if (stringValue is null) return ClothingCategory.NotSet;
            
            if (Enum.TryParse(typeof(ClothingCategory), stringValue, true, out object result))
            {
                return (ClothingCategory)result;
            }
            
            // handle possible variations
            return stringValue.ToLowerInvariant() switch
            {
                _ => ClothingCategory.NotSet,
            };
        }

        public override void Write(Utf8JsonWriter writer, ClothingCategory value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString().ToLowerInvariant());
        
        public static ClothingCategory Parse(string category) 
            => Enum.TryParse(typeof(ClothingCategory), category, true, out object result) ?
                (ClothingCategory)result : ClothingCategory.NotSet;
    }
}