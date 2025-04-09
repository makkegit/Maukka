using Maukka.Models;

namespace Maukka.Pages
{
    public partial class WardrobeDetailPage : ContentPage
    {
        public WardrobeDetailPage(WardrobeDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }

}