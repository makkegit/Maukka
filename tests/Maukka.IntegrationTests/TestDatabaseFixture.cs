using Maukka.Data;

namespace Maukka.IntegrationTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    private SqliteConnection _connection;
    
    public SqliteConnection Connection => _connection;
    
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
    
    
    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:;Pooling=False");
        await _connection.OpenAsync();
        
        // Initialize schema and seed data
        using var command = _connection.CreateCommand();

        foreach (var sqlCmd in _createTableCommands)
        {
            command.CommandText = sqlCmd;
            await command.ExecuteNonQueryAsync();
        }
    }
    
    public SqliteConnection GetConnection() => _connection;

    public virtual async ValueTask DisposeAsync()
    {
        // await _connection.CloseAsync();
        // await _connection.DisposeAsync();
        // GC.SuppressFinalize(this);
    } 
}