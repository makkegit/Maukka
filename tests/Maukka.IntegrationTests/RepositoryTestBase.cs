using Maukka.Data;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;

namespace Maukka.IntegrationTests
{
    public abstract class RepositoryTestBase : IAsyncLifetime
    {
        protected SqliteConnection _connection;
        protected ILogger<WardrobeRepository> _logger;
        protected TestDatabaseFixture _testDatabaseFixture;
        private readonly string[] _tables = 
            [
                "Wardrobes", "Clothing", "Brands", 
                "ClothingSizes", "BrandClothing", 
                "SizeMeasurements", "ClothingXref"
            ];
        public async ValueTask InitializeAsync()
        {
            _testDatabaseFixture = new TestDatabaseFixture();
            _connection = await _testDatabaseFixture.GetConnectionAsync();
            
            // Create a test logger
            var loggerFactory = LoggerFactory.Create(builder => 
                builder.AddXUnit()); 
            _logger = loggerFactory.CreateLogger<WardrobeRepository>();
        }
        
        public async ValueTask DisposeAsync()
        {
            // foreach (var table in _tables)
            // {
            //     await ClearTable(table);
            // }
            //
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
            await _testDatabaseFixture.DisposeAsync();
        }

        private async Task ClearTable(string tableName)
        {
            await using var cmd = _connection.CreateCommand();
            cmd.CommandText = $"DROP TABLE {tableName}";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}