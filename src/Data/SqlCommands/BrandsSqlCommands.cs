namespace Maukka.Data
{
    public static class BrandsSqlCommands
    {
        public const string Insert = "INSERT INTO Brands (BrandName) VALUES (@BrandName); SELECT last_insert_rowid();";
        public const string Update = "UPDATE Brands SET BrandName = @BrandName where BrandId = @BrandId;";
        public const string GetWithBrandId = "SELECT * FROM Brands WHERE BrandId = @BrandId";
    }
}