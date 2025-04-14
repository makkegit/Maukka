namespace Maukka.Data
{
    public static class SizeMeasurementsSQLCommands
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