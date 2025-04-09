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
        [Description("uk")]
        UK = 2,
        [Description("us")]
        US = 3,
        [Description("jp")]
        JP = 4,
        [Description("aus")]
        AUS = 5
    }
}