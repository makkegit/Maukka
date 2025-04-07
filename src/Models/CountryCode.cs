// ReSharper disable InconsistentNaming

using System.ComponentModel;

namespace Maukka.Models
{
    public enum CountryCode
    {
        [Description("unknown")]
        Unknown = 0,
        [Description("eu")]
        EU = 1,
        UK = 2,
        US = 3,
        JP = 4,
        AUS = 5
    }
}