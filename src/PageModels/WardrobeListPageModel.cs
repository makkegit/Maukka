#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Data;
using Maukka.Models;
using Maukka.Services;

namespace Maukka.PageModels
{
    public partial class WardrobeListPageModel : ObservableObject
    {
        private readonly WardrobeRepository _wardrobeRepository;

        [ObservableProperty]
        private List<Wardrobe> _wardrobes = [];
        
        public WardrobeListPageModel(WardrobeRepository wardrobeRepository)
        {
            _wardrobeRepository = wardrobeRepository;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            Wardrobes = await _wardrobeRepository.ListAsync();
        }

        [RelayCommand]
        Task NavigateToWardrobe(Wardrobe wardrobe)
            => Shell.Current.GoToAsync($"wardrobe?id={wardrobe.WardrobeId}");

        [RelayCommand]
        async Task AddWardrobe()
        {
            await Shell.Current.GoToAsync($"wardrobe");
        }
    }
}