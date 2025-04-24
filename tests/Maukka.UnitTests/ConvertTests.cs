using System.Text.Json;
using Maukka.Data;
using Maukka.Models;

namespace Maukka.UnitTests
{
    public class ConvertTests
    {
        private readonly string _wardrobeDataFilePath = "wardrobes.json";
        private readonly string _brandDataFilePath = "brands.json";
        private readonly string _brandClothingDataFilePath = "brandClothing.json";
        private readonly string _clothingSizeDataFilePath = "clothingSizes.json";
        
        
        [Fact]
        public void Convert_ClothingCategory_From_JSON()
        {
            string testDirectory =  Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _wardrobeDataFilePath));

            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.WardrobesJson);
            Assert.NotNull(payload);
            
            // In JSON categories are not parsed
            var allClothes = payload.Wardrobes.SelectMany(wr => wr.Items).ToList();
            Assert.Contains(allClothes, c => c.Category == ClothingCategory.NotSet);
        }

        [Fact]
        public void Convert_Brands_From_JSON()
        {
            string testDirectory =  Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _brandDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandsJson);
            Assert.NotNull(payload);
            
            var allBrands = payload.Brands.ToList();
            Assert.NotEmpty(allBrands);
        }

        [Fact]
        public void Convert_BrandClothing_From_JSON()
        {
            string testDirectory =  Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _brandClothingDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandClothingJson);
            Assert.NotNull(payload);
            
            var allBrandClothing = payload.BrandClothing.ToList();
            Assert.NotEmpty(allBrandClothing);
        }

        [Fact]
        public void Convert_ClothingSize_From_JSON()
        {
            string testDirectory =  Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _clothingSizeDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ClothingSizesJson);
            Assert.NotNull(payload);
            
            var allClothingSizes = payload.ClothingSizes.ToList();
            Assert.NotEmpty(allClothingSizes);

            int i = 1;
            foreach (var size in allClothingSizes)
            {
                size.SizeId = i;
                i++;
            }

            var json = JsonSerializer.Serialize(allClothingSizes);
            Assert.NotNull(json);
        }
    }
}