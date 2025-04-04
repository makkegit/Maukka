using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maukka.Data;
using Maukka.Models;
using Maukka.Services;

namespace Maukka.PageModels
{
    public partial class TaskDetailPageModel : ObservableObject, IQueryAttributable
    {
        public const string ProjectQueryKey = "project";
        private Clothing? _task;
        private bool _canDelete;
        private readonly WardrobeRepository _wardrobeRepository;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _title = string.Empty;

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

        public TaskDetailPageModel(WardrobeRepository wardrobeRepository, ModalErrorHandler errorHandler)
        {
            _wardrobeRepository = wardrobeRepository;
            _errorHandler = errorHandler;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            LoadTaskAsync(query).FireAndForgetSafeAsync(_errorHandler);
        }

        private async Task LoadTaskAsync(IDictionary<string, object> query)
        {
            if (query.TryGetValue(ProjectQueryKey, out var wardrobe))
                Wardrobe = (Wardrobe)wardrobe;

            int taskId = 0;

            if (query.ContainsKey("id"))
            {
                taskId = Convert.ToInt32(query["id"]);
                // _task = await _taskRepository.GetAsync(taskId);
                //
                // if (_task is null)
                // {
                //     _errorHandler.HandleError(new Exception($"Task Id {taskId} isn't valid."));
                //     return;
                // }

                //Wardrobe = await _wardrobeRepository.GetAsync(_task.ProjectID);
            }
            else
            {
                //_task = new Clothing();
            }

            // If the project is new, we don't need to load the project dropdown
            if (Wardrobe?.ID == 0)
            {
                IsExistingProject = false;
            }
            else
            {
                Wardrobes = await _wardrobeRepository.ListAsync();
                IsExistingProject = true;
            }

            if (Wardrobe is not null)
                SelectedProjectIndex = Wardrobes.FindIndex(p => p.ID == Wardrobe.ID);
            // else if (_task?.ProjectID > 0)
            //     SelectedProjectIndex = Wardrobes.FindIndex(p => p.ID == _task.ProjectID);

            if (taskId > 0)
            {
                if (_task is null)
                {
                    _errorHandler.HandleError(new Exception($"Task with id {taskId} could not be found."));
                    return;
                }

                // Title = _task.Title;
                // IsCompleted = _task.IsCompleted;
                CanDelete = true;
            }
            else
            {
                // _task = new Clothing()
                // {
                //     ProjectID = Wardrobe?.ID ?? 0
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
        private async Task Save()
        {
            if (_task is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task or project is null. The task could not be saved."));

                return;
            }

            // _task.Title = Title;

            int projectId = Wardrobe?.ID ?? 0;
            

            if (Wardrobe?.ID == projectId && !Wardrobe.Clothes.Contains(_task))
                Wardrobe.Clothes.Add(_task);

            // if (_task.ProjectID > 0)
            //     _taskRepository.SaveItemAsync(_task).FireAndForgetSafeAsync(_errorHandler);

            await Shell.Current.GoToAsync("..?refresh=true");

            // if (_task.Id > 0)
            //     await AppShell.DisplayToastAsync("Task saved");
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task Delete()
        {
            if (_task is null || Wardrobe is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task is null. The task could not be deleted."));

                return;
            }

            if (Wardrobe.Clothes.Contains(_task))
                Wardrobe.Clothes.Remove(_task);

            // if (_task.ID > 0)
            //     await _taskRepository.DeleteItemAsync(_task);

            await Shell.Current.GoToAsync("..?refresh=true");
            await AppShell.DisplayToastAsync("Task deleted");
        }
    }
}