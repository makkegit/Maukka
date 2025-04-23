#nullable disable
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Data;
using Maukka.Models;
using Maukka.Services;
using Microsoft.Extensions.Logging;

namespace Maukka.PageModels
{
    public partial class WardrobeListPageModel : ObservableObject
    {
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ILogger<WardrobeListPageModel> _logger;

        [ObservableProperty]
        private List<Wardrobe> _wardrobes = [];
        
        public WardrobeListPageModel(WardrobeRepository wardrobeRepository, ILogger<WardrobeListPageModel> logger)
        {
            _wardrobeRepository = wardrobeRepository;
            _logger = logger;
        }
        
        partial void OnWardrobesChanged(List<Wardrobe> value)
        {
            foreach (var wardrobe in value)
            {
                wardrobe.Statistics = WardrobeStatistics.CreateFromClothing(wardrobe.Items);
            }
        }
        
        [RelayCommand]
        private async Task Appearing()
        {
            Wardrobes = await _wardrobeRepository.ListAsync();
        }

        [RelayCommand]
        private async Task NavigateToWardrobe(Wardrobe wardrobe)
        {
            _logger.LogDebug("Navigate to wardrobe with id: {wardrobeId}", wardrobe.WardrobeId);
            await Shell.Current.GoToAsync($"wardrobe?id={wardrobe.WardrobeId.Value}");
        }

        [RelayCommand]
        private async Task AddWardrobe()
        {
            await Shell.Current.GoToAsync($"wardrobe");
        }
    }
}