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

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            await using var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = WardrobeSqlCommands.GetWardrobes;
            var wardrobes = new List<Wardrobe>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var wardrobe = await GetAsync(new WardrobeId
                {
                    Value = reader.GetInt32(0)
                });

                if (wardrobe is not null)
                {
                    wardrobes.Add(wardrobe);
                }
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

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            await using var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = WardrobeSqlCommands.GetWardrobeById;
            selectCmd.Parameters.AddWithValue("@WardrobeId", id.Value);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var wardrobe = Wardrobe.InitWardrobe(
                    reader.GetInt32(reader.GetOrdinal("WardrobeId")),
                    reader.GetString(reader.GetOrdinal("Name")),
                    reader.GetString(reader.GetOrdinal("Description")),
                    []);

                await using var itemsCmd = _connection.CreateCommand();
                itemsCmd.CommandText = ClothingXrefSqlCommands.GetClothingByWardrobeId;
                itemsCmd.Parameters.AddWithValue("@WardrobeId", id.Value);
                await using var readerItems = await itemsCmd.ExecuteReaderAsync();
                while (await readerItems.ReadAsync())
                {
                    var clothing = new Clothing()
                    {
                        ClothingId = readerItems.GetInt32(readerItems.GetOrdinal("ClothingId")),
                        BrandClothingId = readerItems.GetInt32(readerItems.GetOrdinal("BrandClothingId")),
                        ClothingName = readerItems.GetString(readerItems.GetOrdinal("BrandClothingName")),
                        BrandName = readerItems.GetString(readerItems.GetOrdinal("BrandName")),
                        Category = EnumToStringConverter.StringToEnum<ClothingCategory>(readerItems.GetString(readerItems.GetOrdinal("ClothingCategory"))),
                        Alias = readerItems.GetString(readerItems.GetOrdinal("Alias"))
                    };

                    clothing.Size = new ClothingSize(
                        readerItems.GetInt32(readerItems.GetOrdinal("SizeId")),
                        readerItems.GetInt32(readerItems.GetOrdinal("BrandId")),
                        EnumToStringConverter.StringToEnum<CountryCode>(readerItems.GetString(readerItems.GetOrdinal("CountryCode"))),
                        EnumToStringConverter.StringToEnum<MeasurementUnit>(readerItems.GetString(readerItems.GetOrdinal("Unit"))),
                        clothing.Category,
                        readerItems.GetString(readerItems.GetOrdinal("SizeCode")),
                        readerItems.GetInt32(readerItems.GetOrdinal("AgeFromMonths")),
                        readerItems.GetInt32(readerItems.GetOrdinal("AgeToMonths"))
                    );

                    await RetrieveMeasurements(clothing.Size);

                    wardrobe.AddClothing(clothing);
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

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            await using var transaction = _connection.BeginTransaction();
            try
            {
                await using var sqlCmd = _connection.CreateCommand();
                sqlCmd.Transaction = transaction;

                sqlCmd.CommandText = WardrobeSqlCommands.GetSingleById;
                sqlCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId.Value);

                var queryResult = await sqlCmd.ExecuteScalarAsync().ConfigureAwait(false);

                sqlCmd.CommandText = queryResult is not null
                    ? sqlCmd.CommandText = WardrobeSqlCommands.Update
                    : sqlCmd.CommandText = WardrobeSqlCommands.Insert;

                sqlCmd.Parameters.AddWithValue("@Name", item.Name);
                sqlCmd.Parameters.AddWithValue("@Description", item.Description);

                var result = await sqlCmd.ExecuteScalarAsync().ConfigureAwait(false);

                await transaction.CommitAsync();

                if (item.WardrobeId == 0)
                {
                    item.WardrobeId = Convert.ToInt32(result);
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                Console.WriteLine(e);
                throw;
            }

            await AddClothingXref(item).ConfigureAwait(false);

            return item.WardrobeId;
        }

        private async Task AddClothing(List<Clothing> clothing)
        {
            try
            {
                foreach (var cloth in clothing)
                {
                    await SaveItemAsync(cloth).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task AddClothingXref(Wardrobe item)
        {
            await AddClothing(item.Items).ConfigureAwait(false);

            await using var transaction = _connection.BeginTransaction();
            try
            {
                await using var sqlCmd = _connection.CreateCommand();
                sqlCmd.Transaction = transaction;

                foreach (var clothing in item.Items)
                {
                    sqlCmd.CommandText = ClothingXrefSqlCommands.Get;
                    sqlCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId.Value);
                    sqlCmd.Parameters.AddWithValue("@ClothingId", clothing.ClothingId.Value);
                    var result = await sqlCmd.ExecuteScalarAsync();

                    if (result is null)
                    {
                        sqlCmd.CommandText = ClothingXrefSqlCommands.Insert;
                        await sqlCmd.ExecuteNonQueryAsync();
                    }

                    sqlCmd.Parameters.Clear();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ClothingId> SaveItemAsync(Clothing item)
        {
            await InitAsync().ConfigureAwait(false);

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            await using var transaction = _connection.BeginTransaction();
            try
            {
                await using var saveCmd = _connection.CreateCommand();
                saveCmd.Transaction = transaction;
                saveCmd.CommandText = ClothingSqlCommands.GetById;

                saveCmd.Parameters.AddValues(item);
                var existing = await saveCmd.ExecuteScalarAsync();

                saveCmd.CommandText = existing is null ||
                                      item.ClothingId.Value == 0
                    ? ClothingSqlCommands.Insert
                    : ClothingSqlCommands.Update;


                var result = await saveCmd.ExecuteScalarAsync();

                if (item.ClothingId == 0)
                {
                    item.ClothingId = Convert.ToInt32(result);
                }
                await transaction.CommitAsync();

                return item.ClothingId;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
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
            deleteCmd.CommandText = "DELETE FROM Wardrobes WHERE WardrobeId = @WardrobeId";
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
            dropCmd.CommandText = "DROP TABLE IF EXISTS Wardrobes";
            await dropCmd.ExecuteNonQueryAsync();

            _hasBeenInitialized = false;
        }

        #region Brands

        public async Task<BrandId> SaveItem(Brand brand)
        {
            await InitAsync().ConfigureAwait(false);
            await _connection.OpenAsync().ConfigureAwait(false);

            await using var transaction = _connection.BeginTransaction();

            try
            {
                await using var saveCmd = _connection.CreateCommand();
                saveCmd.Transaction = transaction;
                saveCmd.CommandText = BrandsSqlCommands.GetWithBrandId;
                saveCmd.Parameters.AddWithValue("@BrandId", brand.BrandId.Value);
                var brandResult = await saveCmd.ExecuteScalarAsync();

                saveCmd.CommandText = brand.BrandId == 0 || !Convert.ToBoolean(brandResult)
                    ? BrandsSqlCommands.Insert
                    : BrandsSqlCommands.Update;

                saveCmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
                var result = await saveCmd.ExecuteScalarAsync();

                if (brand.BrandId == 0)
                {
                    brand.BrandId = Convert.ToInt32(result);
                }

                await transaction.CommitAsync();

                return brand.BrandId;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
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

            try
            {
                await using var selectCmd = _connection.CreateCommand();
                selectCmd.CommandText = BrandsSqlCommands.GetWithBrandId;
                selectCmd.Parameters.AddWithValue("@BrandId", id.Value);

                await using var reader = await selectCmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return null;
                }

                var brand = new Brand(reader.GetInt32(0), reader.GetString(1));

                return brand;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                await using var saveCmd = _connection.CreateCommand();
                saveCmd.Transaction = transaction;
                saveCmd.CommandText = ClothingSizeSqlCommands.SizeIdsCount;

                saveCmd.Parameters.AddWithValue("@SizeId", clothingSize.SizeId);

                var foundRows = await saveCmd.ExecuteScalarAsync();

                saveCmd.CommandText = Convert.ToInt32(foundRows) == 0
                    ? ClothingSizeSqlCommands.Insert
                    : ClothingSizeSqlCommands.Update;


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
                    saveCmd.CommandText = SizeMeasurementsSqlCommands.GetAllWithKey;

                    saveCmd.Parameters.AddWithValue("@SizeId", sizeId ?? clothingSize.SizeId);
                    saveCmd.Parameters.AddWithValue("@MeasurementKey", key);

                    var sizeMeasureResult = await saveCmd.ExecuteScalarAsync();

                    saveCmd.CommandText = sizeMeasureResult is null
                        ? SizeMeasurementsSqlCommands.Insert
                        : SizeMeasurementsSqlCommands.Update;

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


        public async Task<List<ClothingSize>> GetClothingSizes(BrandId brandId)
        {
            await InitAsync().ConfigureAwait(false);

            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            try
            {
                await using var selectCmd = _connection.CreateCommand();
                selectCmd.CommandText = ClothingSizeSqlCommands.GetByBrandId;
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

                    await RetrieveMeasurements(clothingSize);

                    clothingSizes.Add(clothingSize);
                }

                return clothingSizes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task RetrieveMeasurements(ClothingSize clothingSize)
        {
            await using var measurementCmd = _connection.CreateCommand();
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
            await using var transaction = _connection.BeginTransaction();

            try
            {
                await using var saveCmd = _connection.CreateCommand();
                saveCmd.Transaction = transaction;

                saveCmd.Parameters.AddValues(
                    brandClothing.BrandClothingId,
                    brandClothing.Brand.BrandId,
                    brandClothing.Name,
                    brandClothing.Category);

                saveCmd.CommandText = BrandClothingSqlCommands.GetSingleById;

                var existing = await saveCmd.ExecuteScalarAsync();

                saveCmd.CommandText = existing is null ||
                                      brandClothing.BrandClothingId == 0
                    ? BrandClothingSqlCommands.Insert
                    : BrandClothingSqlCommands.Update;

                var result = await saveCmd.ExecuteScalarAsync();

                if (brandClothing.BrandClothingId == 0)
                {
                    brandClothing.BrandClothingId = Convert.ToInt32(result);
                }

                await transaction.CommitAsync();

                return brandClothing.BrandClothingId;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<BrandClothing>> GetClothingByBrandId(BrandId brandId)
        {
            await InitAsync().ConfigureAwait(false);
            if (_connection.State is not ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }

            try
            {
                await using var getCmd = _connection.CreateCommand();

                if (brandId.Value == 0)
                {
                    return [];
                }

                var foundClothing = new List<BrandClothing>();

                getCmd.CommandText = BrandClothingSqlCommands.GetByBrandId;
                getCmd.Parameters.AddWithValue("@BrandId", brandId.Value);

                var reader = await getCmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var brandClothing = new BrandClothing()
                    {
                        BrandClothingId = reader.GetInt32(reader.GetOrdinal("BrandClothingId")),
                        Brand = new Brand(reader.GetInt32(reader.GetOrdinal("BrandId")), reader.GetString(reader.GetOrdinal("BrandName"))),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Category = EnumToStringConverter.StringToEnum<ClothingCategory>(reader.GetString(reader.GetOrdinal("Category"))),
                        ClothingSizes = [],
                    };

                    brandClothing.ClothingSizes = await GetClothingSizes(brandClothing.Brand.BrandId);

                    foundClothing.Add(brandClothing);
                }

                return foundClothing;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

    }
}