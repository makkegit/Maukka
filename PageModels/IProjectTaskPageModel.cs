using CommunityToolkit.Mvvm.Input;
using Maukka.Models;

namespace Maukka.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}