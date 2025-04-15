using Maukka.Data;

namespace Maukka.IntegrationTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    protected string _testDatabasePath;
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

    public async Task<SqliteConnection> GetConnectionAsync()
    {
        _testDatabasePath = Path.Combine(Path.GetTempPath(), 
            $"test.db3");
        
        var connection = new SqliteConnection($"Data Source={_testDatabasePath};Pooling=False");
        await connection.OpenAsync();
        
        // Initialize schema and seed data
        using var command = connection.CreateCommand();

        foreach (var sqlCmd in _createTableCommands)
        {
            command.CommandText = sqlCmd;
            await command.ExecuteNonQueryAsync();
        }
        
        return connection;
    }
    

    public async ValueTask InitializeAsync() { await ValueTask.CompletedTask; }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDatabasePath))
        {
            try { File.Delete(_testDatabasePath); }
            catch (Exception e) { Console.WriteLine(e); throw; }
        }
    } 
}