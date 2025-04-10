using System.Text.Json;
using Maukka.Models;
using Microsoft.Extensions.Logging;

namespace Maukka.Data
{
    public class SeedDataService
    {
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly string _seedDataFilePath = "SeedData.json";
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(WardrobeRepository wardrobeRepository, ILogger<SeedDataService> logger)
        {
            _wardrobeRepository = wardrobeRepository;
            _logger = logger;
        }

        public async Task LoadSeedDataAsync()
        {
            ClearTables();

            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_seedDataFilePath);

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

        private async void ClearTables()
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