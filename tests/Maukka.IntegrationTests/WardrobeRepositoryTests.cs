using System.Text.Json;
using Maukka.Data;
using Maukka.Models;
using Maukka.Utilities.Converters;

namespace Maukka.IntegrationTests
{
    public class WardrobeRepositoryTests : RepositoryTestBase
    {
        private readonly string _wardrobeDataFilePath = "wardrobes.json";
        private readonly string _brandDataFilePath = "brands.json";
        private readonly string _brandClothingDataFilePath = "brandClothing.json";
        private readonly string _clothingSizeDataFilePath = "clothingSizes.json";

        private WardrobeRepository CreateRepository() =>
            new(_logger, _connection);

        
        [Fact]
        public async Task InitTestData_Successfully()
        {
            var repository = CreateRepository();
            await InitTestData(repository);

            var brand = await repository.GetAsync(new BrandId(1));
            Assert.NotNull(brand);
            
            var clothingSizes = await repository.GetClothingSizes(brand.BrandId);
            Assert.NotEmpty(clothingSizes);
            
            var brandClothing = await repository.GetClothingByBrandId(new BrandId(1));
            Assert.NotEmpty(brandClothing);

            var wardrobes = await repository.ListAsync();
            Assert.NotEmpty(wardrobes);
        }
        
        private async Task InitTestData(WardrobeRepository repository)
        {
            await InitBrands(repository);
            await InitClothingSizes(repository);
            await InitBrandClothing(repository);
            await InitWardrobes(repository);
        }

        private async Task InitWardrobes(WardrobeRepository repository)
        {
            string testDirectory = Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _wardrobeDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.WardrobesJson);
            foreach (var wardrobe in payload.Wardrobes)
            {
                await repository.SaveItemAsync(wardrobe);
            }
        }

        private async Task InitBrands(WardrobeRepository repository)
        {
            string testDirectory = Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _brandDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandsJson);
            foreach (var brand in payload.Brands)
            {
                await repository.SaveItem(brand);
            }
        }

        private async Task InitClothingSizes(WardrobeRepository repository)
        {
            var testDirectory = Directory.GetCurrentDirectory();
            await using var templateStream = File.OpenRead(Path.Combine(testDirectory, _clothingSizeDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ClothingSizesJSON);

            foreach (var clothingSize in payload.ClothingSizes)
            {
                await repository.SaveItem(clothingSize);
            }
        }

        private async Task InitBrandClothing(WardrobeRepository repository)
        {
            var testDirectory = Directory.GetCurrentDirectory();
            await using var templateStream = File.OpenRead(Path.Combine(testDirectory, _brandClothingDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandClothingJson);

            foreach (var brandClothing in payload.BrandClothing)
            {
                await repository.SaveItem(brandClothing);
            }
        }
        
    }
}