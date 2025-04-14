using System.ComponentModel;

namespace Maukka.Models
{
    public enum MeasurementUnit
    {
        [Description("centimeters")]
        Cm,
        [Description("millimeters")]
        Mm,
        [Description("inches")]
        Inch,
    }
}