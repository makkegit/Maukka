using Maukka.Models;
using Microsoft.Data.Sqlite;

namespace Maukka.Data.SqlCommands
{
    public static class ClothingSqlCommands
    {
        public const string Insert = @"INSERT INTO Clothing
                                     (BrandClothingId, SizeId, Alias, Quantity) 
                                      VALUES (@BrandClothingId, @SizeId, @Alias, @Quantity);
                                      SELECT last_insert_rowid();";

        public const string Update = @"UPDATE Clothing SET SizeId = @SizeId, Alias = @Alias, Quantity = @Quantity
                                       WHERE ClothingId = @ClothingId;";
        
        public const string GetById = "SELECT * FROM Clothing WHERE ClothingId = @ClothingId;";

        public static void AddValues(this SqliteParameterCollection parameters,
            Clothing clothing)
        {
            parameters.Clear();
            
            parameters.AddWithValue("@ClothingId", clothing.ClothingId.Value);
            parameters.AddWithValue("@BrandClothingId", clothing.BrandClothingId.Value);
            parameters.AddWithValue("@SizeId", clothing.Size.SizeId);
            parameters.AddWithValue("@Alias", clothing.Alias);
            parameters.AddWithValue("@Quantity", clothing.Quantity > 0 ? clothing.Quantity : 1);
        }
    }
}