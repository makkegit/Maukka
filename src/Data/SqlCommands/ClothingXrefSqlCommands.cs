namespace Maukka.Data.SqlCommands
{
    public static class ClothingXrefSqlCommands
    {
        public const string DeleteWithWardrobeId = "DELETE FROM ClothingXref WHERE WardrobeId = @WardrobeId;";
        public const string Insert = "INSERT INTO ClothingXref (WardrobeId, ClothingId) VALUES (@WardrobeId, @ClothingId);";
        public const string Update = "UPDATE ClothingXref SET WardrobeId = @WardrobeId WHERE WardrobeId = @WardrobeId AND ClothingId = @ClothingId;";
        public const string Get = "SELECT * FROM ClothingXref WHERE WardrobeId = @WardrobeId AND ClothingId = @ClothingId;";

        public const string GetClothingByWardrobeId = @"
                    SELECT c.ClothingId,c.BrandClothingId,c.Alias, bc.Name AS BrandClothingName,
                            bc.Category as ClothingCategory, b.BrandName, s.*, c.Quantity
                    FROM ClothingXref as X
                         INNER JOIN Clothing c on x.ClothingId = c.ClothingId
                         INNER JOIN BrandClothing bc on c.BrandClothingId = bc.BrandClothingId
                         INNER JOIN Brands b on bc.BrandId = b.BrandId
                         INNER JOIN ClothingSizes s on s.SizeId = c.SizeId
                    WHERE x.WardrobeId = @WardrobeId;";
    }
}