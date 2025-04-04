namespace Maukka.Data
{
    public static class WardrobeSqlCommand
    {
        public const string InitialCreate = @"
            CREATE TABLE IF NOT EXISTS Wardrobes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Icon TEXT NOT NULL,
                CategoryID INTEGER NOT NULL
            );";
        public const string GetWardrobes = "SELECT * FROM Wardrobes"; 
    }
}