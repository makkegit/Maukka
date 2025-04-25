#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Models;
using Microsoft.Extensions.Logging;

namespace Maukka.PageModels
{
    public partial class ClothingListPageModel : ObservableObject
    {
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ILogger<ClothingListPageModel> _logger;

        [ObservableProperty]
        private List<Clothing> _clothing = [];
        
        public ClothingListPageModel(WardrobeRepository wardrobeRepository, ILogger<ClothingListPageModel> logger)
        {
            _wardrobeRepository = wardrobeRepository;
            _logger = logger;
        }
        
        
        // [RelayCommand]
        // private async Task Appearing()
        // {
        //     Clothing = await _wardrobeRepository.ListAsync();
        // }

        [RelayCommand]
        private async Task NavigateToClothing(Clothing clothing)
        {
            _logger.LogDebug("Navigate to clothing with id: {clothingId}", clothing.ClothingId);
            await Shell.Current.GoToAsync($"details?id={clothing.ClothingId.Value}");
        }

        [RelayCommand]
        private async Task AddClothing()
        {
            await Shell.Current.GoToAsync($"clothing");
        }
    }
}