using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Models;

namespace Maukka.PageModels
{
    public partial class ClothingDetailPageModel : ObservableObject, IQueryAttributable
    {
        public const string WardrobeQueryKey = "wardrobe";
        private Clothing? _clothing;
        private bool _canDelete;
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ModalErrorHandler _errorHandler;
        
        [ObservableProperty]
        private string _clothingName = string.Empty;

        [ObservableProperty]
        private string _brandName = string.Empty;

        [ObservableProperty]
        private string _alias = string.Empty;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private ClothingCategory _category;

        [ObservableProperty]
        private ClothingSize _size = new ClothingSize();
        
        public ObservableCollection<MeasurementItem> Measurements { get; } = new();

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private List<Wardrobe> _wardrobes = [];

        [ObservableProperty]
        private Wardrobe? _wardrobe;

        [ObservableProperty]
        private int _selectedProjectIndex = -1;

        [ObservableProperty]
        private bool _isExistingProject;
        
        public ClothingDetailPageModel(WardrobeRepository wardrobeRepository, ModalErrorHandler errorHandler)
        {
            _wardrobeRepository = wardrobeRepository;
            _errorHandler = errorHandler;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            LoadClothingAsync(query).FireAndForgetSafeAsync(_errorHandler);
        }

        private async Task LoadClothingAsync(IDictionary<string, object> query)
        {
            if (query.TryGetValue(WardrobeQueryKey, out var wardrobe))
                Wardrobe = (Wardrobe)wardrobe;

            ClothingId clothingId = 0;

            if (query.ContainsKey("id"))
            {
                clothingId = new ClothingId
                {
                    Value = Convert.ToInt32(query["id"])
                };

                _clothing = await _wardrobeRepository.GetAsync(clothingId);

                if (_clothing is null)
                {
                    _errorHandler.HandleError(new Exception($"ClothingId {clothingId} isn't valid."));
                    return;
                }

                ClothingName = _clothing.ClothingName;
                Quantity = _clothing.Quantity;
                Category = _clothing.Category;
                Size = _clothing.Size;
                foreach (var (key, value) in _clothing.Size.Measurements)
                {
                    Measurements.Add(new(key, value));
                }
                BrandName = _clothing.BrandName;
                Alias = _clothing.Alias;

                //Wardrobe = await _wardrobeRepository.GetAsync(_clothing.ProjectID);
            }
            else
            {
                //_clothing = new Items();
            }

            // If the project is new, we don't need to load the project dropdown
            if (Wardrobe?.WardrobeId == 0)
            {
                IsExistingProject = false;
            }
            else
            {
                Wardrobes = await _wardrobeRepository.ListAsync();
                IsExistingProject = true;
            }

            if (Wardrobe is not null)
                SelectedProjectIndex = Wardrobes.FindIndex(p => p.WardrobeId == Wardrobe.WardrobeId);
            // else if (_clothing?.ProjectID > 0)
            //     SelectedProjectIndex = Wardrobes.FindIndex(p => p.WardrobeId == _clothing.ProjectID);

            if (clothingId.Value > 0)
            {
                if (_clothing is null)
                {
                    _errorHandler.HandleError(new Exception($"Task with id {clothingId} could not be found."));
                    return;
                }

                // Title = _clothing.Title;
                // IsCompleted = _clothing.IsCompleted;
                CanDelete = true;
            }
            else
            {
                // _clothing = new Items()
                // {
                //     ProjectID = Wardrobe?.WardrobeId ?? 0
                // };
            }
        }

        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private void IncrementQuantity()
        {
            if (Quantity < 100)
            {
                Quantity++;
            }
        }

        [RelayCommand]
        private void DecrementQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (_clothing is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task or project is null. The task could not be saved."));

                return;
            }

            // _clothing.Title = Title;

            int projectId = Wardrobe?.WardrobeId.Value ?? 0;


            if (Wardrobe?.WardrobeId == projectId && !Wardrobe.Items.Contains(_clothing))
                Wardrobe.Items.Add(_clothing);

            // if (_clothing.ProjectID > 0)
            //     _taskRepository.SaveItemAsync(_clothing).FireAndForgetSafeAsync(_errorHandler);

            await Shell.Current.GoToAsync("..?refresh=true");

            // if (_clothing.WardrobeId > 0)
            //     await AppShell.DisplayToastAsync("Task saved");
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task Delete()
        {
            if (_clothing is null || Wardrobe is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task is null. The task could not be deleted."));

                return;
            }

            if (Wardrobe.Items.Contains(_clothing))
                Wardrobe.Items.Remove(_clothing);

            // if (_clothing.WardrobeId > 0)
            //     await _taskRepository.DeleteItemAsync(_clothing);

            await Shell.Current.GoToAsync("..?refresh=true");
            await AppShell.DisplayToastAsync("Task deleted");
        }
    }
}