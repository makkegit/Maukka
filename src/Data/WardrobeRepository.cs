using System.Text.Json;
using Maukka.Models;
using Maukka.Utilities.Converters;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Maukka.Data
{
    /// <summary>
    /// Repository class for managing wardrobes in the database.
    /// </summary>
    public class WardrobeRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        private readonly string[] _createTableCommands =
        [
            WardrobeSqlCommands.CreateWardrobesTable,
            WardrobeSqlCommands.CreateClothingTable,
            WardrobeSqlCommands.CreateClothingXrefTable,
            WardrobeSqlCommands.CreateBrandTable,
            WardrobeSqlCommands.CreateBrandClothingTable,
            WardrobeSqlCommands.CreateClothingSizesTable,
            WardrobeSqlCommands.CreateSizeMeasurementsTable
        ];

        /// <summary>
        /// Initializes a new instance of the <see cref="WardrobeRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public WardrobeRepository(ILogger<WardrobeRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the Wardrobe table if it does not exist.
        /// </summary>
        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync().ConfigureAwait(false);
            var createTableCmd = connection.CreateCommand();

            try
            {
                foreach (var createCommand in _createTableCommands)
                {
                    createTableCmd.CommandText = createCommand;
                    await createTableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tables in command: {commandText}", createTableCmd.CommandText);
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all wardrobes from the database.
        /// </summary>
        /// <returns>A list of <see cref="Wardrobe"/> objects.</returns>
        public async Task<List<Wardrobe>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Wardrobe";
            var wardrobes = new List<Wardrobe>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                wardrobes.Add(new Wardrobe
                {
                    WardrobeId = reader.GetInt32(0),
                    Description = reader.GetString(2)
                });
            }

            return wardrobes;
        }

        /// <summary>
        /// Retrieves a specific wardrobe by its WardrobeId.
        /// </summary>
        /// <param name="id">The WardrobeId of the wardrobe.</param>
        /// <returns>A <see cref="Wardrobe"/> object if found; otherwise, null.</returns>
        public async Task<Wardrobe?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = WardrobeSqlCommands.GetWardrobes;
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var wardrobe = new Wardrobe
                {
                    WardrobeId = reader.GetInt32(0),
                    Description = reader.GetString(1)
                };

                if (!reader.IsDBNull(2))
                {
                    wardrobe.Items.Add(new()
                    {
                        ClothingId = reader.GetInt32(2),
                        BrandName = reader.GetString(3),
                        ClothingName = reader.GetString(4),
                        Category = ClothingCategoryConverter.Parse(reader.GetString(5)),
                        Size = JsonSerializer.Deserialize<ClothingSize>(reader.GetString(6)),
                        Alias = reader.GetString(7),
                    });
                }

                return wardrobe;
            }

            return null;
        }

        /// <summary>
        /// Saves a wardrobe to the database. If the wardrobe WardrobeId is 0, a new wardrobe is created; otherwise, the existing wardrobe is updated.
        /// </summary>
        /// <param name="item">The wardrobe to save.</param>
        /// <returns>The WardrobeId of the saved wardrobe.</returns>
        public async Task<WardrobeId> SaveItemAsync(Wardrobe item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            saveCmd.CommandText =
                $"INSERT INTO Wardrobe ({nameof(Wardrobe.Description)})" +
                "VALUES (@Description);" +
                "SELECT last_insert_rowid();";
            var queryCmd = connection.CreateCommand();
            queryCmd.CommandText =
                "SELECT COUNT(*) FROM Wardrobe WHERE WardrobeId = @WardrobeId;";
            queryCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId.Value);

            var queryResult = await queryCmd.ExecuteReaderAsync();
            var hasResults = await queryResult.ReadAsync();

            if (hasResults)
            {
                saveCmd.CommandText = @"
                UPDATE Wardrobe
                SET Description = @Description
                WHERE WardrobeId = @WardrobeId;";
                saveCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId.Value);
            }

            saveCmd.Parameters.AddWithValue("@Description", item.Description);

            var result = await saveCmd.ExecuteScalarAsync();

            await AddClothing(item.Items, saveCmd);
            await AddClothingXref(item, saveCmd, result);

            if (item.WardrobeId == 0)
            {
                item.WardrobeId = Convert.ToInt32(result);
            }

            return item.WardrobeId;
        }

        private async Task AddClothing(List<Clothing> clothing, SqliteCommand saveCmd)
        {
            try
            {
                foreach (var cl in clothing)
                {
                    saveCmd.Parameters.Clear();
                    saveCmd.CommandText =
                        @$"INSERT INTO {nameof(Clothing)} 
                    ({nameof(Clothing.ClothingId)},
                     {nameof(Clothing.BrandClothingId)},  
                     {nameof(Clothing.SizeId)}, 
                     {nameof(Clothing.Alias)})
                VALUES (@ClothingId, @BrandClothingId, @SizeId, @Alias);";

                    saveCmd.Parameters.AddWithValue("@ClothingId", cl.ClothingId.Value);
                    saveCmd.Parameters.AddWithValue("@BrandClothingId", cl.BrandClothingId.Value);
                    saveCmd.Parameters.AddWithValue("@SizeId", cl.SizeId);
                    saveCmd.Parameters.AddWithValue("@Alias", cl.Alias);

                    var result = await saveCmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task AddClothingXref(Wardrobe item, SqliteCommand saveCmd, object? result)
        {
            saveCmd.Parameters.Clear();
            saveCmd.CommandText =
                "DELETE FROM ClothingXref WHERE WardrobeId = @WardrobeId;";
            saveCmd.Parameters.AddWithValue("@WardrobeId",
                item.WardrobeId.Value != 0 ? item.WardrobeId.Value : Convert.ToInt32(result));
            var affectedRows = await saveCmd.ExecuteNonQueryAsync();
            foreach (var clothing in item.Items)
            {
                saveCmd.CommandText =
                    "INSERT INTO ClothingXref (WardrobeId, ClothingId) " +
                    "VALUES (@WardrobeId, @ClothingId);";
                saveCmd.Parameters.AddWithValue("@ClothingId", clothing.ClothingId.Value);
                var clothingXrefResult = await saveCmd.ExecuteScalarAsync();
            }
        }

        public async Task<ClothingId> SaveItemAsync(Clothing item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ClothingId.Value == 0)
            {
                saveCmd.CommandText =
                    "INSERT INTO Clothing " +
                    $"({nameof(Clothing.BrandName)},{nameof(Clothing.ClothingName)}," +
                    $"{nameof(Clothing.Category)},{nameof(Clothing.Size)},{nameof(Clothing.Alias)})" +
                    "VALUES (@BrandName, @ClothingName, @Category, @Size, @Alias);" +
                    "SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE Clothing
                SET BrandName = @BrandName, ClothingName = @ClothingName, Category = @Category, Size = @Size, Alias = @Alias
                WHERE ClothingId = @ClothingId";
                saveCmd.Parameters.AddWithValue("@ClothingId", item.ClothingId.Value);
            }

            saveCmd.Parameters.AddWithValue("@BrandName", item.BrandName);
            saveCmd.Parameters.AddWithValue("@ClothingName", item.ClothingName);
            saveCmd.Parameters.AddWithValue("@Category",
                EnumToStringConverter.EnumToString(item.Category));
            saveCmd.Parameters.AddWithValue("@Size", JsonSerializer.Serialize(item.Size));
            saveCmd.Parameters.AddWithValue("@Alias", item.Alias);

            var result = await saveCmd.ExecuteScalarAsync();

            if (item.ClothingId == 0)
            {
                item.ClothingId = Convert.ToInt32(result);
            }

            return item.ClothingId;
        }

        /// <summary>
        /// Deletes a wardrobe from the database.
        /// </summary>
        /// <param name="item">The wardrobe to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(Wardrobe item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Wardrobe WHERE WardrobeId = @WardrobeId";
            deleteCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId.Value);
            var affectedRows = await deleteCmd.ExecuteNonQueryAsync();

            deleteCmd.CommandText = "DELETE FROM ClothingXref WHERE WardrobeId = @WardrobeId";
            affectedRows += await deleteCmd.ExecuteNonQueryAsync();

            return affectedRows;
        }

        /// <summary>
        /// Drops the Wardrobe table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropCmd = connection.CreateCommand();
            dropCmd.CommandText = "DROP TABLE IF EXISTS Wardrobe";
            await dropCmd.ExecuteNonQueryAsync();

            _hasBeenInitialized = false;
        }

        #region Brands

        public async Task<BrandId> SaveItem(Brand brand)
        {
            await Init().ConfigureAwait(false);
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync().ConfigureAwait(false);

            var saveCmd = connection.CreateCommand();

            if (brand.BrandId == 0)
            {
                saveCmd.CommandText =
                    "INSERT INTO Brands (BrandName) " +
                    "VALUES (@BrandName);" +
                    "SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText =
                    "UPDATE Brands SET BrandName = @BrandName WHERE BrandId = @BrandId;";
                saveCmd.Parameters.AddWithValue("@BrandId", brand.BrandId.Value);
            }

            saveCmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
            var result = await saveCmd.ExecuteScalarAsync();

            if (brand.BrandId == 0)
            {
                brand.BrandId = Convert.ToInt32(result);
            }

            return brand.BrandId;
        }

        #endregion

        #region ClothingSizes

        /// <summary>
        /// Saves the clothing size to database. if sizeId is 0, new value is added
        /// </summary>
        /// <param name="clothingSize">The clothing size to save.</param>
        public async Task SaveItem(ClothingSize clothingSize)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var saveCmd = connection.CreateCommand();
                saveCmd.Transaction = transaction;
                saveCmd.CommandText = ClothingSizeSQLCommands.SizeIdsCount;
                
                saveCmd.Parameters.AddWithValue("@SizeId", clothingSize.SizeId);

                var foundRows = await saveCmd.ExecuteScalarAsync();

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

                var sizeId = await saveCmd.ExecuteScalarAsync();

                foreach (var (key, value) in clothingSize.Measurements)
                {
                    saveCmd.Parameters.Clear();
                    saveCmd.CommandText = SizeMeasurementsSQLCommands.GetAllWithKey;

                    saveCmd.Parameters.AddWithValue("@SizeId", sizeId ?? clothingSize.SizeId);
                    saveCmd.Parameters.AddWithValue("@MeasurementKey", key);

                    var sizeMeasureResult = await saveCmd.ExecuteScalarAsync();

                    saveCmd.CommandText = sizeMeasureResult is null
                        ? SizeMeasurementsSQLCommands.Insert
                        : SizeMeasurementsSQLCommands.Update;

                    saveCmd.Parameters.AddWithValue("@Value", value);

                    await saveCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region BrandClothing

        public async Task<BrandClothingId> SaveItem(BrandClothing brandClothing)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (brandClothing.BrandClothingId == 0)
            {
                saveCmd.CommandText =
                    "INSERT INTO BrandClothing (BrandId, Name, Category) " +
                    "VALUES (@BrandId, @Name, @Category);";
            }
            else
            {
                saveCmd.CommandText =
                    "UPDATE BrandClothing " +
                    "SET BrandId = @BrandId, Name = @Name, Category = @Category" +
                    "WHERE BrandClothingId = @BrandClothingId;";
                saveCmd.Parameters.AddWithValue("@BrandClothingId", brandClothing.BrandClothingId.Value);
            }

            saveCmd.Parameters.AddWithValue("@BrandId", brandClothing.Brand.BrandId.Value);
            saveCmd.Parameters.AddWithValue("@Name", brandClothing.Name);
            saveCmd.Parameters.AddWithValue("@Category",
                EnumToStringConverter.EnumToString(brandClothing.Category));

            var result = await saveCmd.ExecuteScalarAsync();

            if (brandClothing.BrandClothingId == 0)
            {
                brandClothing.BrandClothingId = Convert.ToInt32(result);
            }

            return brandClothing.BrandClothingId;
        }

        #endregion

    }
}