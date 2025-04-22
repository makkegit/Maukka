using Maukka.Models;

namespace Maukka.Data
{
    public static class WardrobeSqlCommands
    {
        public const string Insert = "INSERT INTO Wardrobes (Name, Description) VALUES (@Name, @Description); SELECT last_insert_rowid();";
        public const string Update = "UPDATE Wardrobes SET Name = @Name, Description = @Description WHERE WardrobeId = @WardrobeId;";
        public const string GetSingleById = "SELECT * FROM Wardrobes WHERE WardrobeId = @WardrobeId LIMIT 1;";
        
        public const string CreateWardrobesTable =
            @"CREATE TABLE IF NOT EXISTS Wardrobes (
            WardrobeId INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
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
            @"SELECT w.*
                FROM Wardrobes AS w
                LEFT JOIN ClothingXref as x ON w.WardrobeId = x.WardrobeId";

        public const string GetWardrobeById = "SELECT * FROM Wardrobes WHERE WardrobeId = @WardrobeId;";

    }
}