using Maukka.Data;

namespace Maukka.IntegrationTests;

public class TestDatabaseFixture : IDisposable
{
    private SqliteConnection _connection;
    
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
    
    
    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        
        // Initialize schema and seed data
        using var command = _connection.CreateCommand();

        foreach (var sqlCmd in _createTableCommands)
        {
            command.CommandText = sqlCmd;
            command.ExecuteNonQuery();
        }
    }
    
    public SqliteConnection GetConnection() => _connection;

    public void Dispose()
    {
        _connection.Close();
        _connection?.Dispose();
        _connection = null;
    } 
}