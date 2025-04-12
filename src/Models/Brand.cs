using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Brand
    {
        [JsonConverter(typeof(BrandIdConverter))]
        public BrandId BrandId { get; set; }
        public string BrandName { get; set; }
        

        public Brand(BrandId brandId, string brandName)
        {
            BrandId = brandId;
            BrandName = brandName;
        }
    }

    public class BrandsJSON
    {
        public List<Brand> Brands { get; set; } = [];
    }
}