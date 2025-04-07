using CommunityToolkit.Mvvm.Input;
using Maukka.Models;

namespace Maukka.PageModels
{
    public interface IWardrobePageModel
    {
        IAsyncRelayCommand<Clothing> NavigateToClothingCommand { get; }
        bool IsBusy { get; }
    }
}