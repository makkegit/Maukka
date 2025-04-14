namespace Maukka.Utilities.Converters
{
    public static class EnumToStringConverter
    {
        public static string EnumToString<T>(T value) where T : Enum
        {
            return value.ToString();
        }

        public static T StringToEnum<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, true, out T result)) // Case-insensitive
            {
                return result;
            }
            return default; 
        }
    }
}