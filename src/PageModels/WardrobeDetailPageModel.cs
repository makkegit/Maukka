using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Models;

namespace Maukka.PageModels
{
    public partial class WardrobeDetailPageModel : ObservableObject, IQueryAttributable, IWardrobePageModel
    {
        private Wardrobe? _wardrobe;
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private List<Clothing> _items = [];

        // [ObservableProperty]
        // private List<Category> _categories = [];
        //
        // [ObservableProperty]
        // private Category? _category;

        [ObservableProperty]
        private int _categoryIndex = -1;

        // [ObservableProperty]
        // private List<Tag> _allTags = [];

        [ObservableProperty]
        private string _icon = FluentUI.ribbon_24_regular;

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        private List<string> _icons =
        [
            FluentUI.ribbon_24_regular,
            FluentUI.ribbon_star_24_regular,
            FluentUI.trophy_24_regular,
            FluentUI.badge_24_regular,
            FluentUI.book_24_regular,
            FluentUI.people_24_regular,
            FluentUI.bot_24_regular
        ];

        public bool HasCompletedTasks
            => true;

        public WardrobeDetailPageModel(WardrobeRepository wardrobeRepository, ModalErrorHandler errorHandler)
        {
            _wardrobeRepository = wardrobeRepository;
            _errorHandler = errorHandler;
            Items = [];
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("id"))
            {
                int id = Convert.ToInt32(query["id"]);
                LoadData(id).FireAndForgetSafeAsync(_errorHandler);
            }
            else if (query.ContainsKey("refresh"))
            {
                RefreshData().FireAndForgetSafeAsync(_errorHandler);
            }
            else
            {
                Task.WhenAll(LoadCategories(), LoadTags()).FireAndForgetSafeAsync(_errorHandler);
                _wardrobe = new();
                _wardrobe.Items = [];
                Items = _wardrobe.Items;
            }
        }

        private async Task LoadCategories() => await Task.CompletedTask;
            // Categories = await _categoryRepository.ListAsync();

        private async Task LoadTags() => await Task.CompletedTask;
            // AllTags = await _tagRepository.ListAsync();

        private async Task RefreshData()
        {
            if (_wardrobe.IsNullOrNew())
            {
                if (_wardrobe is not null)
                    Items = new(_wardrobe.Items);

                return;
            }

            //Items = await _taskRepository.ListAsync(_wardrobe.Id);
            _wardrobe.Items = Items;
        }

        private async Task LoadData(int id)
        {
            try
            {
                IsBusy = true;

                _wardrobe = await _wardrobeRepository.GetAsync(id);

                if (_wardrobe.IsNullOrNew())
                {
                    _errorHandler.HandleError(new Exception($"Wardrobe with id {id} could not be found."));
                    return;
                }
                
                Description = _wardrobe.Description;
                Items = _wardrobe.Items;

                // Categories = await _categoryRepository.ListAsync();
                // Category = Categories?.FirstOrDefault(c => c.Id == _wardrobe.CategoryID);
                // CategoryIndex = Categories?.FindIndex(c => c.Id == _wardrobe.CategoryID) ?? -1;
                //
                // var allTags = await _tagRepository.ListAsync();
                // foreach (var tag in allTags)
                // {
                //     tag.IsSelected = _wardrobe.Tags.Any(t => t.Id == tag.Id);
                // }
                // AllTags = new(allTags);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        [RelayCommand]
        private async Task TaskCompleted(Clothing task)
        {
            //await _taskRepository.SaveItemAsync(task);
            OnPropertyChanged(nameof(HasCompletedTasks));
        }


        [RelayCommand]
        private async Task Save()
        {
            if (_wardrobe is null)
            {
                _errorHandler.HandleError(
                    new Exception("Wardrobe is null. Cannot Save."));

                return;
            }
            
            _wardrobe.Description = Description;
            await _wardrobeRepository.SaveItemAsync(_wardrobe);

            if (_wardrobe.IsNullOrNew())
            {
                // foreach (var tag in AllTags)
                // {
                //     if (tag.IsSelected)
                //     {
                //         await _tagRepository.SaveItemAsync(tag, _wardrobe.Id);
                //     }
                // }
            }

            foreach (var task in _wardrobe.Items)
            {
                // if (task.Id == 0)
                // {
                //     task.ProjectID = _wardrobe.Id;
                //     await _taskRepository.SaveItemAsync(task);
                // }
            }

            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Wardrobe saved");
        }

        [RelayCommand]
        private async Task AddTask()
        {
            if (_wardrobe is null)
            {
                _errorHandler.HandleError(
                    new Exception("Wardrobe is null. Cannot navigate to task."));

                return;
            }

            // Pass the project so if this is a new project we can just add
            // the tasks to the project and then save them all from here.
            await Shell.Current.GoToAsync($"task",
                new ShellNavigationQueryParameters(){
                    {TaskDetailPageModel.ProjectQueryKey, _wardrobe}
                });
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (_wardrobe.IsNullOrNew())
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            await _wardrobeRepository.DeleteItemAsync(_wardrobe);
            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Wardrobe deleted");
        }

        [RelayCommand]
        private Task NavigateToClothing(Clothing clothing) =>
            Shell.Current.GoToAsync($"clothing?id={clothing.Id}");

        // [RelayCommand]
        // private async Task ToggleTag(Tag tag)
        // {
        //     tag.IsSelected = !tag.IsSelected;
        //
        //     if (!_wardrobe.IsNullOrNew())
        //     {
        //         if (tag.IsSelected)
        //         {
        //             // await _tagRepository.SaveItemAsync(tag, _wardrobe.Id);
        //         }
        //         else
        //         {
        //             // await _tagRepository.DeleteItemAsync(tag, _wardrobe.Id);
        //         }
        //     }
        //
        //     AllTags = new(AllTags);
        // }

        [RelayCommand]
        private async Task CleanClothing()
        {
            var completedTasks = Items.ToArray();
            // foreach (var task in completedTasks)
            // {
            //     await _taskRepository.DeleteItemAsync(task);
            //     Items.Remove(task);
            // }

            Items = new(Items);
            OnPropertyChanged(nameof(HasCompletedTasks));
            await AppShell.DisplayToastAsync("All cleaned up!");
        }
    }
}
