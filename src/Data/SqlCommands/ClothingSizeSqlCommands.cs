using Maukka.Models;
using Maukka.Utilities.Converters;
using Microsoft.Data.Sqlite;

namespace Maukka.Data.SqlCommands
{
    public static class ClothingSizeSqlCommands
    {
        public const string Insert =
            @"INSERT INTO ClothingSizes
                            (BrandId, CountryCode, Unit, Category, SizeCode, AgeFromMonths, AgeToMonths) 
                            VALUES (@BrandId, @CountryCode, @Unit, @Category, @SizeCode, @AgeFromMonths, @AgeToMonths);
                            SELECT last_insert_rowid();";

        public const string Update =
            @"UPDATE ClothingSizes
                        SET BrandId = @BrandId,
                            CountryCode = @CountryCode,
                            Unit = @Unit,
                            Category = @Category,
                            SizeCode = @SizeCode,
                            AgeFromMonths = @AgeFromMonths,
                            AgeToMonths = @AgeToMonths
                        WHERE SizeId = @SizeId;";
        
        public const string GetById = "SELECT * FROM ClothingSizes WHERE SizeId = @SizeId;";
        public const string GetByBrandId = "SELECT * FROM ClothingSizes WHERE BrandId = @BrandId ORDER BY SizeId ASC;";
        public const string SizeIdsCount = "SELECT COUNT(*) FROM ClothingSizes WHERE SizeId = @SizeId;";

        public static void AddValues(this SqliteParameterCollection parameters,
            int sizeId, BrandId brandId, CountryCode countryCode, MeasurementUnit unit, ClothingCategory category, string sizeCode, int ageFromMonths, int ageToMonths)
        {
            parameters.Clear();
            parameters.AddWithValue("@SizeId", sizeId);
            parameters.AddWithValue("@BrandId", brandId.Value);
            parameters.AddWithValue("@CountryCode",
                EnumToStringConverter.EnumToString(countryCode));
            parameters.AddWithValue("@Unit",
                EnumToStringConverter.EnumToString(unit));
            parameters.AddWithValue("@Category",
                EnumToStringConverter.EnumToString(category));
            parameters.AddWithValue("@SizeCode", sizeCode);
            parameters.AddWithValue("@AgeFromMonths", ageFromMonths);
            parameters.AddWithValue("@AgeToMonths", ageToMonths);
        }
    }
}