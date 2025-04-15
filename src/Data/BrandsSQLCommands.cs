namespace Maukka.Data
{
    public static class BrandsSQLCommands
    {
        public const string GetWithBrandId = "SELECT * FROM Brands WHERE BrandId = @BrandId";
    }
}