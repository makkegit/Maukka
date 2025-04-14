using Maukka.Models;

namespace Maukka.Data
{
    public static class WardrobeSqlCommands
    {
        public const string CreateWardrobesTable =
            @$"CREATE TABLE IF NOT EXISTS {nameof(Wardrobe)} (
            {nameof(Wardrobe.WardrobeId)} INTEGER PRIMARY KEY AUTOINCREMENT,
            {nameof(Wardrobe.Description)} TEXT NOT NULL)";

        public const string CreateClothingTable =
            @$"CREATE TABLE IF NOT EXISTS {nameof(Clothing)} (
            {nameof(Clothing.ClothingId)} INTEGER PRIMARY KEY AUTOINCREMENT, 
            {nameof(Clothing.BrandClothingId)} INTEGER NOT NULL, 
            {nameof(Clothing.SizeId)} INTEGER NOT NULL, 
            {nameof(Clothing.Alias)} TEXT NOT NULL,
            FOREIGN KEY({nameof(Clothing.BrandClothingId)}) REFERENCES {nameof(BrandClothing)}({nameof(BrandClothing.BrandClothingId)}),
            FOREIGN KEY({nameof(Clothing.SizeId)}) REFERENCES ClothingSizes({nameof(ClothingSize.SizeId)})
            );";

        public const string CreateClothingXrefTable =
            @$"CREATE TABLE IF NOT EXISTS ClothingXref (
            {nameof(Wardrobe.WardrobeId)} INTEGER NOT NULL,
            {nameof(Clothing.ClothingId)} INTEGER NOT NULL,
            PRIMARY KEY({nameof(Wardrobe.WardrobeId)}, {nameof(Clothing.ClothingId)}),
            FOREIGN KEY({nameof(Wardrobe.WardrobeId)}) REFERENCES {nameof(Wardrobe)}({nameof(Wardrobe.WardrobeId)}),
            FOREIGN KEY({nameof(Clothing.ClothingId)}) REFERENCES {nameof(Clothing)}({nameof(Clothing.ClothingId)}) 
            );";

        public const string CreateBrandClothingTable =
            @$"CREATE TABLE IF NOT EXISTS BrandClothing (
            {nameof(BrandClothing.BrandClothingId)} INTEGER PRIMARY KEY AUTOINCREMENT,
            {nameof(Brand.BrandId)} INTEGER NOT NULL,
            {nameof(BrandClothing.Name)} TEXT NOT NULL,
            {nameof(BrandClothing.Category)} TEXT NOT NULL, 
            FOREIGN KEY({nameof(Brand.BrandId)}) REFERENCES Brands({nameof(Brand.BrandId)})
            );";

        public const string CreateBrandTable =
            @$"CREATE TABLE IF NOT EXISTS Brands (
            {nameof(Brand.BrandId)} INTEGER PRIMARY KEY AUTOINCREMENT,
            {nameof(Brand.BrandName)} TEXT NOT NULL);";

        public const string CreateClothingSizesTable =
            @"CREATE TABLE IF NOT EXISTS ClothingSizes (
            SizeId INTEGER PRIMARY KEY AUTOINCREMENT,
            BrandId INTEGER NOT NULL,
            CountryCode TEXT NOT NULL CHECK(upper(CountryCode) IN ('US', 'EU', 'UK', 'JP')),
            Unit TEXT NOT NULL CHECK(upper(Unit) IN ('CM', 'INCH')),
            Category TEXT NOT NULL,
            SizeCode TEXT,
            AgeFromMonths INTEGER,
            AgeToMonths INTEGER,
            FOREIGN KEY(BrandId) REFERENCES Brands(BrandId)
            );";

        public const string CreateSizeMeasurementsTable =
            @"CREATE TABLE IF NOT EXISTS SizeMeasurements (
            SizeId INTEGER NOT NULL,
            MeasurementKey TEXT NOT NULL CHECK(lower(MeasurementKey) IN ('height', 'chest', 'sizenbr', 'waist', 'hips', 'insideleglength')),
            Value REAL NOT NULL,
            PRIMARY KEY (SizeId, MeasurementKey),
            FOREIGN KEY(SizeId) REFERENCES ClothingSizes(SizeId)
            )";

        public const string GetWardrobes =
            @$"SELECT w.*, c.*
            FROM {nameof(Wardrobe.WardrobeId)} AS w
            LEFT JOIN ClothingXref x ON w.{nameof(Wardrobe.WardrobeId)} = x.{nameof(Wardrobe.WardrobeId)}
            LEFT JOIN {nameof(Clothing)} c ON x.{nameof(Clothing.ClothingId)} = c.{nameof(Clothing.ClothingId)}
            WHERE w.WardrobeId = @id;";

        
    }
}