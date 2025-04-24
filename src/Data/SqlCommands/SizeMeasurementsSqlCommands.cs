namespace Maukka.Data.SqlCommands
{
    public static class SizeMeasurementsSqlCommands
    {
        public const string GetAllWithKey = @"SELECT * FROM SizeMeasurements 
                            WHERE SizeId = @SizeId AND MeasurementKey = @MeasurementKey;";

        public const string Insert = @"INSERT INTO SizeMeasurements (SizeId, MeasurementKey, Value)
                                        VALUES (@SizeId, @MeasurementKey, @Value);";
        
        public const string Update = @"UPDATE SizeMeasurements 
                                        SET Value = @Value 
                                        WHERE SizeId = @SizeId AND MeasurementKey = @MeasurementKey;";
    }
}