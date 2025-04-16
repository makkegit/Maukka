namespace Maukka.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "Maukka.db3";
        public static string DatabaseConnectionString =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";
    }
}