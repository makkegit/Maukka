using System.Text.Json;
using Maukka.Models;
using Font = Microsoft.Maui.Font;

namespace Maukka.UnitTests
{
    public class ConvertTests
    {
        private readonly string _seedDataFilePath = "wardrobes.json";
        
        [Fact]
        public void Convert_ClothingCategory_From_JSON()
        {
            string testDirectory =  Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _seedDataFilePath));

            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.WardrobesJson);
            Assert.NotNull(payload);
            
            var allClothes = payload.Wardrobes.SelectMany(wr => wr.Clothes).ToList();
            Assert.DoesNotContain(allClothes, c => c.ClothingCategory == ClothingCategory.NotSet);
        }
    }
}