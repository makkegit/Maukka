using Maukka.Models;

namespace Maukka.Pages
{
    public partial class WardrobeDetailPage : ContentPage
    {
        public WardrobeDetailPage(ProjectDetailPageModel model)
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
            return (item as Tag)?.IsSelected ?? false ? SelectedTagTemplate : NormalTagTemplate;
        }
    }
}