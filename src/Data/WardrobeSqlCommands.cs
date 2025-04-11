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
            $"{nameof(Clothing.ClothingId)} INTEGER PRIMARY KEY AUTOINCREMENT, " +
            $"{nameof(Clothing.BrandName)} TEXT NOT NULL, " +
            $"{nameof(Clothing.ClothingName)} TEXT NOT NULL, " +
            $"{nameof(Clothing.Category)} TEXT NOT NULL, " +
            $"{nameof(Clothing.Size)} TEXT NOT NULL, " +
            $"{nameof(Clothing.Alias)} TEXT NOT NULL)";

        public const string CreateClothingXrefTable =
            "CREATE TABLE IF NOT EXISTS ClothingXref (" +
            $"{nameof(Wardrobe.WardrobeId)} INTEGER," +
            $"{nameof(Clothing.ClothingId)} INTEGER," +
            $"PRIMARY KEY({nameof(Wardrobe.WardrobeId)}, {nameof(Clothing.ClothingId)})," +
            $"FOREIGN KEY({nameof(Wardrobe.WardrobeId)}) REFERENCES {nameof(Wardrobe)}({nameof(Wardrobe.WardrobeId)})," +
            $"FOREIGN KEY({nameof(Clothing.ClothingId)}) REFERENCES {nameof(Clothing)}({nameof(Clothing.ClothingId)}));";
        
        public const string CreateBrandClothingTable =
            "CREATE TABLE IF NOT EXISTS BrandClothing (" +
            $"{nameof(BrandClothing.BrandClothingId)} INTEGER," +
            $"{nameof(Brand.BrandId)} INTEGER," +
            $"{nameof(BrandClothing.Name)} TEXT NOT NULL," +
            $"{nameof(BrandClothing.Category)} TEXT NOT NULL, " +
            $"PRIMARY KEY({nameof(BrandClothing.BrandClothingId)})," +
            $"FOREIGN KEY({nameof(Brand.BrandId)}) REFERENCES {nameof(Brand)}({nameof(Brand.BrandId)});";

        public const string CreateBrandTable =
            "CREATE TABLE IF NOT EXISTS Brand (" +
            $"{nameof(Brand.BrandId)} INTEGER," +
            $"{nameof(Brand.BrandName)} TEXT NOT NULL);";
        
        public const string CreateBrandClothingSizesTable =
            "CREATE TABLE IF NOT EXISTS BrandClothingSizes (" +
            "SizeId INTEGER PRIMARY KEY AUTOINCREMENT," +
            "BrandClothingId INTEGER NOT NULL," +
            "CountryCode TEXT NOT NULL CHECK(CountryCode IN ('US', 'EU', 'UK', 'JP'))," +
            "Unit TEXT NOT NULL CHECK(Unit IN ('CM', 'INCH'))," +
            "Category TEXT NOT NULL," +
            "SizeCode TEXT," +
            "AgeFromMonths INTEGER," +
            "AgeToMonths INTEGER," +
            "FOREIGN KEY(BrandClothingId) REFERENCES BrandClothing(BrandClothingId)" +
            ");";

        public const string CreateSizeMeasurementsTable =
            "CREATE TABLE IF NOT EXISTS BrandSizeMeasurements (" +
            "SizeId INTEGER NOT NULL," +
            "MeasurementKey TEXT NOT NULL CHECK(MeasurementKey IN ('chest', 'height', 'sizeNbr', 'waist', 'hips', 'insideLegLength'))," +
            "Value REAL NOT NULL," +
            "PRIMARY KEY (SizeId, MeasurementKey)," +
            "FOREIGN KEY(SizeId) REFERENCES BrandClothingSizes(SizeId)" +
            ");";

        public const string GetWardrobes =
            "SELECT w.*, c.*" +
            $"FROM {nameof(Wardrobe.WardrobeId)}" +
            $"LEFT JOIN ClothingXref x ON w.{nameof(Wardrobe.WardrobeId)} = x.{nameof(Wardrobe.WardrobeId)}" +
            $"LEFT JOIN {nameof(Clothing)} c ON x.{nameof(Clothing.ClothingId)} = c.{nameof(Clothing.ClothingId)}" +
            $"WHERE w.WardrobeId = @id;";
    }
}