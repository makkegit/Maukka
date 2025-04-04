using System.Diagnostics.CodeAnalysis;
using Maukka.Models;

namespace Maukka.Utilities
{
    /// <summary>
    /// Wardrobe Model Extentions
    /// </summary>
    public static class ProjectExtentions
    {
        /// <summary>
        /// Check if the project is null or new.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static bool IsNullOrNew([NotNullWhen(false)] this Wardrobe? project)
        {
            return project is null || project.ID == 0;
        }
    }
}