using Maukka.Models;
using Maukka.Utilities.Converters;
using Microsoft.Data.Sqlite;

namespace Maukka.Data.SqlCommands
{
    public static class BrandClothingSqlCommands
    {
        public const string Insert = "INSERT INTO BrandClothing (BrandId, Name, Category) VALUES (@BrandId, @Name, @Category); SELECT last_insert_rowid();";
        public const string Update = "UPDATE BrandClothing SET Name = @Name, Category = @Category WHERE BrandClothingId = @BrandClothingId;";
        
        public const string GetByBrandId = @"SELECT bc.*, b.BrandName FROM BrandClothing as bc 
                                            LEFT JOIN Brands as b ON b.BrandId = bc.BrandId
                                            WHERE b.BrandId = @BrandId;";

        public const string GetSingleById = @"SELECT bc.*, b.BrandName FROM BrandClothing as bc
                                        LEFT JOIN Brands as b ON b.BrandId = bc.BrandId
                                        WHERE b.BrandId = @BrandId AND bc.BrandClothingId = @BrandClothingId;";

        public const string GetBrandNameById = @"SELECT b.BrandName FROM BrandClothing as bc 
                                                 INNER JOIN Brands as b ON b.BrandId = bc.BrandId
                                                 WHERE b.BrandId = @BrandId;";
        public static void AddValues(this SqliteParameterCollection parameters,
            BrandClothingId brandClothingId, BrandId brandId, string name, ClothingCategory category)
        {
            parameters.Clear();
            parameters.AddWithValue("@BrandClothingId", brandClothingId.Value);
            parameters.AddWithValue("@BrandId", brandId.Value);
            parameters.AddWithValue("@Name", name);
            parameters.AddWithValue("@Category", EnumToStringConverter.EnumToString(category));
        }
    }
}