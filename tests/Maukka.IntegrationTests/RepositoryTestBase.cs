using Maukka.Data;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;

namespace Maukka.IntegrationTests
{
    public abstract class RepositoryTestBase : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
    {
        protected readonly TestDatabaseFixture _fixture;
        protected readonly SqliteConnection _connection;
        protected readonly ILogger<WardrobeRepository> _logger;

        protected RepositoryTestBase(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _connection = _fixture.Connection;
        
            // Create a test logger
            var loggerFactory = LoggerFactory.Create(builder => 
                builder.AddXUnit()); 
            _logger = loggerFactory.CreateLogger<WardrobeRepository>();
        }

        public ValueTask InitializeAsync() => ValueTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            await ClearTable("Wardrobes");
            await ClearTable("Clothing");
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        private async Task ClearTable(string tableName)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {tableName}";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}