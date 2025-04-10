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
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();

                createTableCmd.CommandText = WardrobeSqlCommands.CreateWardrobesTable;
                await createTableCmd.ExecuteNonQueryAsync();
                
                createTableCmd.CommandText = WardrobeSqlCommands.CreateClothingTable;
                
                await createTableCmd.ExecuteNonQueryAsync();
                
                createTableCmd.CommandText = WardrobeSqlCommands.CreateClothingXrefTable;
                    
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating Wardrobe table");
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
            if (item.WardrobeId == 0)
            {
                saveCmd.CommandText =
                $"INSERT INTO Wardrobe ({nameof(Wardrobe.Description)})"+
                "VALUES (@Description);" +
                "SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE Wardrobe
                SET Description = @Description
                WHERE WardrobeId = @WardrobeId";
                saveCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId);
            }
            saveCmd.Parameters.AddWithValue("@Description", item.Description);

            var result = await saveCmd.ExecuteScalarAsync();
            if (item.WardrobeId == 0)
            {
                item.WardrobeId = Convert.ToInt32(result);
            }

            return item.WardrobeId;
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
            deleteCmd.Parameters.AddWithValue("@WardrobeId", item.WardrobeId);

            return await deleteCmd.ExecuteNonQueryAsync();
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
    }
}