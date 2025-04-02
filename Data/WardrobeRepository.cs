using Maukka.Models;
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
                createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Wardrobe (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Icon TEXT NOT NULL,
                CategoryID INTEGER NOT NULL
            );";
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
                    ID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Icon = reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                });
            }

            return wardrobes;
        }

        /// <summary>
        /// Retrieves a specific wardrobe by its ID.
        /// </summary>
        /// <param name="id">The ID of the wardrobe.</param>
        /// <returns>A <see cref="Wardrobe"/> object if found; otherwise, null.</returns>
        public async Task<Wardrobe?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Wardrobe WHERE ID = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var wardrobe = new Wardrobe
                {
                    ID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Icon = reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                };
                
                return wardrobe;
            }

            return null;
        }

        /// <summary>
        /// Saves a wardrobe to the database. If the wardrobe ID is 0, a new wardrobe is created; otherwise, the existing wardrobe is updated.
        /// </summary>
        /// <param name="item">The wardrobe to save.</param>
        /// <returns>The ID of the saved wardrobe.</returns>
        public async Task<int> SaveItemAsync(Wardrobe item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                saveCmd.CommandText = @"
                INSERT INTO Wardrobe (Name, Description, Icon, CategoryID)
                VALUES (@Name, @Description, @Icon, @CategoryID);
                SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE Wardrobe
                SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID
                WHERE ID = @ID";
                saveCmd.Parameters.AddWithValue("@ID", item.ID);
            }

            saveCmd.Parameters.AddWithValue("@Name", item.Name);
            saveCmd.Parameters.AddWithValue("@Description", item.Description);
            saveCmd.Parameters.AddWithValue("@Icon", item.Icon);
            saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);

            var result = await saveCmd.ExecuteScalarAsync();
            if (item.ID == 0)
            {
                item.ID = Convert.ToInt32(result);
            }

            return item.ID;
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
            deleteCmd.CommandText = "DELETE FROM Wardrobe WHERE ID = @ID";
            deleteCmd.Parameters.AddWithValue("@ID", item.ID);

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