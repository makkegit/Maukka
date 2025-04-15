using Maukka.Models;

namespace Maukka.Data
{
    public static class WardrobeSqlCommands
    {
        public const string CreateWardrobesTable =
            @"CREATE TABLE IF NOT EXISTS Wardrobes (
            WardrobeId INTEGER PRIMARY KEY AUTOINCREMENT,
            Description TEXT NOT NULL)";

        public const string CreateClothingTable =
            @"CREATE TABLE IF NOT EXISTS Clothing (
                ClothingId INTEGER PRIMARY KEY AUTOINCREMENT, 
                BrandClothingId INTEGER NOT NULL, 
                SizeId INTEGER NOT NULL, 
                Alias TEXT NOT NULL,
                FOREIGN KEY(BrandClothingId) REFERENCES BrandClothing(BrandClothingId),
                FOREIGN KEY(SizeId) REFERENCES ClothingSizes(SizeId)
            );";

        public const string CreateClothingXrefTable =
            @"CREATE TABLE IF NOT EXISTS ClothingXref (
                WardrobeId INTEGER NOT NULL,
                ClothingId INTEGER NOT NULL,
                PRIMARY KEY(WardrobeId, ClothingId),
                FOREIGN KEY(WardrobeId) REFERENCES Wardrobes(WardrobeId),
                FOREIGN KEY(ClothingId) REFERENCES Clothing(ClothingId) 
            );";

        public const string CreateBrandClothingTable =
            @"CREATE TABLE IF NOT EXISTS BrandClothing (
                BrandClothingId INTEGER PRIMARY KEY AUTOINCREMENT,
                BrandId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL, 
                FOREIGN KEY(BrandId) REFERENCES Brands(BrandId)
            );";

        public const string CreateBrandTable =
            @"CREATE TABLE IF NOT EXISTS Brands (
                BrandId INTEGER PRIMARY KEY AUTOINCREMENT,
                BrandName TEXT NOT NULL);";

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
                FROM WardrobeId AS w
                LEFT JOIN ClothingXref x ON w.{nameof(Wardrobe.WardrobeId)} = x.{nameof(Wardrobe.WardrobeId)}
                LEFT JOIN {nameof(Clothing)} c ON x.{nameof(Clothing.ClothingId)} = c.{nameof(Clothing.ClothingId)}
                WHERE w.WardrobeId = @id;";
    }
}