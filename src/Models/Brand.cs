using System.Text.Json.Serialization;
using Maukka.Utilities.Converters;

namespace Maukka.Models
{
    public class Brand
    {
        [JsonConverter(typeof(BrandIdConverter))]
        public BrandId BrandId { get; init; }
        public string BrandName { get; set; }
        

        public Brand(BrandId brandId, string brandName)
        {
            BrandId = brandId;
            BrandName = brandName;
        }
    }
}