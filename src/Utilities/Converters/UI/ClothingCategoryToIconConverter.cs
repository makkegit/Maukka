using System.Globalization;
using Maukka.Models;

namespace Maukka.Utilities.Converters.UI
{
    public class ClothingCategoryToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ClothingCategory category)
            {
                return category switch
                {
                    ClothingCategory.Tops => Fonts.ClothesUI.Shirt,
                    ClothingCategory.Bottoms => Fonts.ClothesUI.Trousers,
                    ClothingCategory.Shoes => Fonts.ClothesUI.Shoe,
                    ClothingCategory.Hats => Fonts.ClothesUI.Hat,
                    _ => Fonts.ClothesUI.tshirt,
                };
            }

            return Fonts.ClothesUI.tshirt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}