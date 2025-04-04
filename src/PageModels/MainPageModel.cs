using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Models;

namespace Maukka.PageModels
{
    public partial class MainPageModel : ObservableObject, IProjectTaskPageModel
    {
        private bool _isNavigatedTo;
        private bool _dataLoaded;
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly SeedDataService _seedDataService;

        // [ObservableProperty]
        // private List<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private List<Clothing> _tasks = [];

        [ObservableProperty]
        private List<Wardrobe> _wardrobes = [];

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, MMM d");

        public bool HasCompletedTasks
            => true;

        public MainPageModel(SeedDataService seedDataService, WardrobeRepository wardrobeRepository,
                ModalErrorHandler errorHandler)
        {
            _wardrobeRepository = wardrobeRepository; ;
            _errorHandler = errorHandler;
            _seedDataService = seedDataService;
        }

        private async Task LoadData()
        {
            try
            {
                IsBusy = true;

                Wardrobes = await _wardrobeRepository.ListAsync();
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        private async Task InitData(SeedDataService seedDataService)
        {
            bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

            if (!isSeeded)
            {
                await seedDataService.LoadSeedDataAsync();
            }

            Preferences.Default.Set("is_seeded", true);
            await Refresh();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadData();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void NavigatedTo() =>
            _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() =>
            _isNavigatedTo = false;

        [RelayCommand]
        private async Task Appearing()
        {
            if (!_dataLoaded)
            {
                await InitData(_seedDataService);
                _dataLoaded = true;
                await Refresh();
            }
            // This means we are being navigated to
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        // [RelayCommand]
        // private Task TaskCompleted(Clothing task)
        // {
        //     OnPropertyChanged(nameof(HasCompletedTasks));
        //     return _taskRepository.SaveItemAsync(task);
        // }
        //
        // [RelayCommand]
        // private Task AddTask()
        //     => Shell.Current.GoToAsync($"task");

        [RelayCommand]
        private Task NavigateToProject(Wardrobe wardrobe)
            => Shell.Current.GoToAsync($"wardrobe?id={wardrobe.ID}");

        [RelayCommand]
        private Task NavigateToTask(Clothing clothing)
            => Shell.Current.GoToAsync($"clothing?id={clothing.BrandClothingID}");

        // [RelayCommand]
        // private async Task CleanTasks()
        // {
        //     var completedTasks = Clothes.Where(t => t.IsCompleted).ToList();
        //     foreach (var task in completedTasks)
        //     {
        //         await _taskRepository.DeleteItemAsync(task);
        //         Clothes.Remove(task);
        //     }
        //
        //     OnPropertyChanged(nameof(HasCompletedTasks));
        //     Clothes = new(Clothes);
        //     await AppShell.DisplayToastAsync("All cleaned up!");
        // }
    }
}