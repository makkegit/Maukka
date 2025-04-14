using System.ComponentModel;
using System.Text.Json;
using Maukka.Models;
using Microsoft.Extensions.Logging;

namespace Maukka.Data
{
    public class SeedDataService
    {
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly string _wardrobeDataFilePath = "wardrobes.json";
        private readonly string _brandDataFilePath = "brands.json";
        private readonly string _brandClothingDataFilePath = "brandClothing.json";
        private readonly string _clothingSizeDataFilePath = "clothingSizes.json";
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(
            WardrobeRepository wardrobeRepository, ILogger<SeedDataService> logger)
        {
            _wardrobeRepository = wardrobeRepository;
            _logger = logger;
        }

        public async Task LoadSeedDataAsync()
        {
            await ClearTables().ConfigureAwait(false);
            await InitSampleData().ConfigureAwait(false);
            
            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_wardrobeDataFilePath);

            WardrobesJson? payload = null;
            try
            {
                payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.WardrobesJson);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deserializing seed data");
            }

            try
            {
                if (payload is not null)
                {
                    foreach (var wardrobe in payload.Wardrobes)
                    {
                        if (wardrobe is null)
                        {
                            continue;
                        }

                        // if (wardrobe.Category is not null)
                        // {
                        //     await _categoryRepository.SaveItemAsync(wardrobe.Category);
                        //     wardrobe.CategoryID = wardrobe.Category.WardrobeId;
                        // }

                        await _wardrobeRepository.SaveItemAsync(wardrobe);

                        // if (wardrobe?.Items is not null)
                        // {
                        //     foreach (var task in wardrobe.Items)
                        //     {
                        //         task.ProjectID = wardrobe.WardrobeId;
                        //         await _taskRepository.SaveItemAsync(task);
                        //     }
                        // }

                        // if (wardrobe?.Tags is not null)
                        // {
                        //     foreach (var tag in wardrobe.Tags)
                        //     {
                        //         await _tagRepository.SaveItemAsync(tag, wardrobe.WardrobeId);
                        //     }
                        // }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving seed data");
                throw;
            }
        }

        private async Task InitSampleData()
        {
            try
            {
                await InitBrandData().ConfigureAwait(false);
                await InitClothingSizeData().ConfigureAwait(false);
                await InitBrandClothing().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private async Task InitBrandClothing()
        {
            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_brandClothingDataFilePath);
            var brandClothing = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandClothingJson);

            foreach (var bc in brandClothing.BrandClothing)
            {
                await _wardrobeRepository.SaveItem(bc);
            }
        }
        private async Task InitClothingSizeData()
        {
            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_clothingSizeDataFilePath);
            var clothingSizes = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ClothingSizesJSON);

            foreach (var clothingSize in clothingSizes.ClothingSizes)
            {
                await _wardrobeRepository.SaveItem(clothingSize);
            }
        }
        private async Task InitBrandData()
        {
            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_brandDataFilePath);
            var brands = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandsJson);

            foreach (var brand in brands.Brands)
            {
                await _wardrobeRepository.SaveItem(brand).ConfigureAwait(false);
            }
        }

        private async Task ClearTables()
        {
            try
            {
                await Task.WhenAll(
                    _wardrobeRepository.DropTableAsync());
                // _taskRepository.DropTableAsync(),
                // _tagRepository.DropTableAsync(),
                // _categoryRepository.DropTableAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}