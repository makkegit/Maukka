namespace Maukka.Pages
{
    public partial class ClothingDetailPage : ContentPage
    {
        public ClothingDetailPage(ClothingDetailPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}