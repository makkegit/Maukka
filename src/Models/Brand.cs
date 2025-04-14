using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Brand(BrandId brandId, string brandName)
    {
        [JsonConverter(typeof(BrandIdConverter))]
        public BrandId BrandId { get; set; } = brandId;

        public string BrandName { get; set; } = brandName;
    }

    public class BrandsJson
    {
        public List<Brand> Brands { get; set; } = [];
    }
}