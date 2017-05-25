using System.Text.RegularExpressions;

namespace EasyDbLib
{
    public class NameChecker
    {
        private static string tableNamePattern = "^[a-z_ ]+$"; // @" ^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$";
        private static string propertyNamePattern = @"^@?[a-zA-Z_]\w*(\.@?[a-zA-Z_]\w*)*$";

        public static bool CheckTableName(string tableName)
        {
            return new Regex(tableNamePattern).IsMatch(tableName);
        }

        public static bool CheckPropertyName(string propertyName)
        {
            return new Regex(propertyNamePattern).IsMatch(propertyName);
        }

    }
}
