using System.Text.Json;
using Maukka.Data;
using Maukka.Utilities.Converters;

namespace Maukka.IntegrationTests
{
    public class WardrobeRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _database;
        private readonly string _wardrobeDataFilePath = "wardrobes.json";
        private readonly string _brandDataFilePath = "brands.json";
        private readonly string _brandClothingDataFilePath = "brandClothing.json";
        private readonly string _clothingSizeDataFilePath = "clothingSizes.json";

        public WardrobeRepositoryTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        private void InitBrands(SqliteConnection connection)
        {
            string testDirectory = Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _brandDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.BrandsJson);

            foreach (var brand in payload.Brands)
            {
                using var saveCmd = connection.CreateCommand();
                saveCmd.Parameters.Clear();
                saveCmd.CommandText = "INSERT INTO Brands (BrandName) VALUES (@BrandName)";
                saveCmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
                saveCmd.ExecuteNonQuery();
            }
        }

        [Fact]
        public void UpsertClothingSizes()
        {
            var connection = _database.GetConnection();
            InitBrands(connection);
            string testDirectory = Directory.GetCurrentDirectory();
            using var templateStream = File.OpenRead(Path.Combine(testDirectory, _clothingSizeDataFilePath));
            var payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ClothingSizesJSON);

            
            using var transaction = connection.BeginTransaction();
            using var saveCmd = connection.CreateCommand();
            saveCmd.Transaction = transaction;
            
            for (int i = 0; i < 2; i++)
            {
                foreach (var clothingSize in payload.ClothingSizes)
                {
                    saveCmd.Parameters.Clear();
                    saveCmd.CommandText = ClothingSizeSQLCommands.SizeIdsCount;
                
                    saveCmd.Parameters.AddWithValue("@SizeId", clothingSize.SizeId);

                    var foundRows = saveCmd.ExecuteScalar();

                    saveCmd.CommandText = Convert.ToInt32(foundRows) == 0
                        ? ClothingSizeSQLCommands.InsertClothingSize
                        : ClothingSizeSQLCommands.UpdateClothingSize;


                    saveCmd.Parameters.AddValues(
                        clothingSize.SizeId,
                        clothingSize.BrandId,
                        clothingSize.CountryCode,
                        clothingSize.MeasurementUnit,
                        clothingSize.Category,
                        clothingSize.SizeCode,
                        clothingSize.AgeToMonths,
                        clothingSize.AgeFromMonths);

                    var sizeId = saveCmd.ExecuteScalar();

                    foreach (var (key, value) in clothingSize.Measurements)
                    {
                        saveCmd.Parameters.Clear();
                        saveCmd.CommandText = SizeMeasurementsSQLCommands.GetAllWithKey;

                        saveCmd.Parameters.AddWithValue("@SizeId", sizeId ?? clothingSize.SizeId);
                        saveCmd.Parameters.AddWithValue("@MeasurementKey", key);

                        var sizeMeasureResult = saveCmd.ExecuteScalar();

                        saveCmd.CommandText = sizeMeasureResult is null
                            ? SizeMeasurementsSQLCommands.Insert
                            : SizeMeasurementsSQLCommands.Update;

                        saveCmd.Parameters.AddWithValue("@Value", value);

                        saveCmd.ExecuteNonQuery();
                    }
                }
            }

            transaction.Rollback();
        }
    }
}