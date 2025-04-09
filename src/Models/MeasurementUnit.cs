using System.ComponentModel;

namespace Maukka.Models
{
    public enum MeasurementUnit
    {
        [Description("cm")]
        Centimeter,
        [Description("mm")]
        Millimeter,
        [Description("in")]
        Inch,
    }
}