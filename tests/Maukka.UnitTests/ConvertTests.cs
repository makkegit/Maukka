using System.Text.Json;
using Maukka.Models;

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
            
            var allClothes = payload.Wardrobes.SelectMany(wr => wr.Items).ToList();
            Assert.DoesNotContain(allClothes, c => c.Category == ClothingCategory.NotSet);
        }
    }
}