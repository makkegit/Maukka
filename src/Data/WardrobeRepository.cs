using System.Data;
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
        private readonly SqliteConnection _connection;
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
        /// <param name="connection">The SQLite connection</param>
        public WardrobeRepository(ILogger<WardrobeRepository> logger,
            SqliteConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        /// <summary>
        /// Initializes the database connection and creates the Wardrobe table if it does not exist.
        /// </summary>
        private async Task InitAsync()
        {
            if (_hasBeenInitialized) return;

            var createTableCmd = _connection.CreateCommand();

            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);

                foreach (var createCommand in _createTableCommands)
                {
                    createTableCmd.CommandText = createCommand;
                    await createTableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tables with command: {commandText}", createTableCmd.CommandText);
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
            await InitAsync().ConfigureAwait(false);

            await _connection.OpenAsync().ConfigureAwait(false);

            var selectCmd = _connection.CreateCommand();
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
        public async Task<Wardrobe?> GetAsync(WardrobeId id)
        {
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var selectCmd = _connection.CreateCommand();
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
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var saveCmd = _connection.CreateCommand();

            saveCmd.CommandText =
                $"INSERT INTO Wardrobe ({nameof(Wardrobe.Description)})" +
                "VALUES (@Description);" +
                "SELECT last_insert_rowid();";
            var queryCmd = _connection.CreateCommand();
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
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var saveCmd = _connection.CreateCommand();
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
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var deleteCmd = _connection.CreateCommand();
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
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var dropCmd = _connection.CreateCommand();
            dropCmd.CommandText = "DROP TABLE IF EXISTS Wardrobe";
            await dropCmd.ExecuteNonQueryAsync();

            _hasBeenInitialized = false;
        }

        #region Brands

        public async Task<BrandId> SaveItem(Brand brand)
        {
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var saveCmd = _connection.CreateCommand();
            saveCmd.CommandText = "SELECT * FROM Brands WHERE BrandId = @BrandId";
            saveCmd.Parameters.AddWithValue("@BrandId", brand.BrandId.Value);
            var brandResult = await saveCmd.ExecuteScalarAsync();

            if (brand.BrandId == 0 ||
                !Convert.ToBoolean(brandResult))
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
            }

            saveCmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
            var result = await saveCmd.ExecuteScalarAsync();

            if (brand.BrandId == 0)
            {
                brand.BrandId = Convert.ToInt32(result);
            }

            return brand.BrandId;
        }

        /// <summary>
        /// Retrieves a specific brand by its WardrobeId.
        /// </summary>
        /// <param name="id">The WardrobeId of the brand.</param>
        /// <returns>A <see cref="Brand"/> object if found; otherwise, null.</returns>
        public async Task<Brand?> GetAsync(BrandId id)
        {
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = BrandsSQLCommands.GetWithBrandId;
            selectCmd.Parameters.AddWithValue("@BrandId", id.Value);

            await using var reader = await selectCmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var brand = new Brand(reader.GetInt32(0), reader.GetString(1));

            return brand;
        }

        #endregion

        #region ClothingSizes

        /// <summary>
        /// Saves the clothing size to database. if sizeId is 0, new value is added
        /// </summary>
        /// <param name="clothingSize">The clothing size to save.</param>
        public async Task SaveItem(ClothingSize clothingSize)
        {
            await InitAsync().ConfigureAwait(false);

            if (_connection.State is ConnectionState.Closed)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }
            
            await using var transaction = _connection.BeginTransaction();

            try
            {
                var saveCmd = _connection.CreateCommand();
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


        public async Task<IReadOnlyCollection<ClothingSize>> GetClothingSizes(BrandId brandId)
        {
            await InitAsync().ConfigureAwait(false);

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            await using var selectCmd = _connection.CreateCommand();

            selectCmd.CommandText = ClothingSizeSQLCommands.GetByBrandId;
            selectCmd.Parameters.AddWithValue("@BrandId", brandId.Value);
            await using var reader = await selectCmd.ExecuteReaderAsync();
            
            var clothingSizes = new List<ClothingSize>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var clothingSize = new ClothingSize
                {
                    SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                    BrandId = new BrandId(reader.GetInt32(reader.GetOrdinal("BrandId"))),
                    CountryCode = EnumToStringConverter.StringToEnum<CountryCode>(reader.GetString(reader.GetOrdinal("CountryCode"))),
                    MeasurementUnit = EnumToStringConverter.StringToEnum<MeasurementUnit>(reader.GetString(reader.GetOrdinal("Unit"))),
                    Category = EnumToStringConverter.StringToEnum<ClothingCategory>(reader.GetString(reader.GetOrdinal("Category"))),
                    SizeCode = reader.GetString(reader.GetOrdinal("SizeCode")),
                    AgeFromMonths = reader.GetInt32(reader.GetOrdinal("AgeFromMonths")),
                    AgeToMonths = reader.GetInt32(reader.GetOrdinal("AgeToMonths")),
                    Measurements = new Dictionary<string, float>()
                };
                
                // Retrieve measurements
                await RetrieveMeasurements(clothingSize);

                clothingSizes.Add(clothingSize);
            }
            
            return clothingSizes.AsReadOnly();
        }

        private async Task RetrieveMeasurements(ClothingSize clothingSize)
        {
            using var measurementCmd = _connection.CreateCommand();
            measurementCmd.CommandText = @"
                    SELECT MeasurementKey, Value 
                    FROM SizeMeasurements 
                    WHERE SizeId = @SizeId";

            measurementCmd.Parameters.AddWithValue("@SizeId", clothingSize.SizeId);

            await using var measurementReader = await measurementCmd.ExecuteReaderAsync().ConfigureAwait(false);

            while (await measurementReader.ReadAsync().ConfigureAwait(false))
            {
                var key = measurementReader.GetString(
                    measurementReader.GetOrdinal("MeasurementKey"));
                var value = (float)measurementReader.GetDouble(
                    measurementReader.GetOrdinal("Value"));

                clothingSize.Measurements.Add(key.ToLower(), value);
            }
        }

        #endregion

        #region BrandClothing

        public async Task<BrandClothingId> SaveItem(BrandClothing brandClothing)
        {
            await InitAsync().ConfigureAwait(false);

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            var saveCmd = _connection.CreateCommand();

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