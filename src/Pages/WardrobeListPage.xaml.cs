namespace Maukka.Pages
{
    public partial class WardrobeListPage : ContentPage
    {
        public WardrobeListPage(ProjectListPageModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}