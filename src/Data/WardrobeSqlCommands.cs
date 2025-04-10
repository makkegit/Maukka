using Maukka.Models;

namespace Maukka.Data
{
    public static class WardrobeSqlCommands
    {
        public const string CreateWardrobesTable =
            $"CREATE TABLE IF NOT EXISTS {nameof(Wardrobe)} (" +
            $"{nameof(Wardrobe.WardrobeId)} INTEGER PRIMARY KEY AUTOINCREMENT," +
            $"{nameof(Wardrobe.Description)} TEXT NOT NULL)";
        
        public const string CreateClothingTable =
            $"CREATE TABLE IF NOT EXISTS {nameof(Clothing)} (" +
            $"{nameof(Clothing.ClothingId)} INTEGER PRIMARY KEY AUTOINCREMENT," +
            $"{nameof(Clothing.BrandName)} TEXT NOT NULL)" +
            $"{nameof(Clothing.ClothingName)} TEXT NOT NULL)" +
            $"{nameof(Clothing.Category)} TEXT NOT NULL)" +
            $"{nameof(Clothing.Size)} TEXT NOT NULL)" +
            $"{nameof(Clothing.Alias)} TEXT NOT NULL)";
        
        public const string CreateClothingXrefTable =
            "CREATE TABLE IF NOT EXISTS ClothingXref (" +
            $"{nameof(Wardrobe.WardrobeId)} INTEGER," +
            $"{nameof(Clothing.ClothingId)} INTEGER," +
            $"PRIMARY KEY({nameof(Wardrobe.WardrobeId)}, {nameof(Clothing.ClothingId)})," +
            $"FOREIGN KEY({nameof(Wardrobe.WardrobeId)}) REFERENCES {nameof(Wardrobe)}({nameof(Wardrobe.WardrobeId)})," +
            $"FOREIGN KEY({nameof(Clothing.ClothingId)}) REFERENCES {nameof(Clothing)}({nameof(Clothing.ClothingId)}));";
        
        public const string GetWardrobes =
            "SELECT w.*, c.*" +
            $"FROM {nameof(Wardrobe.WardrobeId)}" +
            $"LEFT JOIN ClothingXref x ON w.{nameof(Wardrobe.WardrobeId)} = x.{nameof(Wardrobe.WardrobeId)}" +
            $"LEFT JOIN {nameof(Clothing)} c ON x.{nameof(Clothing.ClothingId)} = c.{nameof(Clothing.ClothingId)}" +
            $"WHERE w.WardrobeId = @id;";
    }
}