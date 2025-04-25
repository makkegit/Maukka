using System.Diagnostics.CodeAnalysis;
using Maukka.Models;

namespace Maukka.Utilities
{
    /// <summary>
    /// Wardrobe Model Extentions
    /// </summary>
    public static class WardrobeExtentions
    {
        /// <summary>
        /// Check if the wardrobe is null or new.
        /// </summary>
        /// <param name="wardrobe"></param>
        /// <returns></returns>
        public static bool IsNullOrNew([NotNullWhen(false)] this Wardrobe? wardrobe)
        {
            return wardrobe is null || wardrobe.WardrobeId == 0;
        }
    }
}