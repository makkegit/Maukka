namespace Maukka.Pages
{
    public partial class ClothingListPage : ContentPage
    {
        public ClothingListPage(ClothingListPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}