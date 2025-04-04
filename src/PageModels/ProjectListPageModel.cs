#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Data;
using Maukka.Models;
using Maukka.Services;

namespace Maukka.PageModels
{
    public partial class ProjectListPageModel : ObservableObject
    {
        private readonly WardrobeRepository _wardrobeRepository;

        [ObservableProperty]
        private List<Wardrobe> _projects = [];

        public ProjectListPageModel(WardrobeRepository wardrobeRepository)
        {
            _wardrobeRepository = wardrobeRepository;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            Projects = await _wardrobeRepository.ListAsync();
        }

        [RelayCommand]
        Task NavigateToProject(Wardrobe wardrobe)
            => Shell.Current.GoToAsync($"wardrobe?id={wardrobe.ID}");

        [RelayCommand]
        async Task AddProject()
        {
            await Shell.Current.GoToAsync($"project");
        }
    }
}