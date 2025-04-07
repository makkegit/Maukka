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

    public class ChipDataTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate SelectedTagTemplate { get; set; }
        public required DataTemplate NormalTagTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return NormalTagTemplate;
        }
    }
}