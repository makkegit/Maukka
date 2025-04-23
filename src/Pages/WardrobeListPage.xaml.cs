namespace Maukka.Pages
{
    public partial class WardrobeListPage : ContentPage
    {
        public WardrobeListPage(WardrobeListPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}