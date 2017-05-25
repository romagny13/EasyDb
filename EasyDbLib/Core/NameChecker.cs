using System.Text.RegularExpressions;

namespace EasyDbLib
{
    public class NameChecker
    {
        private static string propertyNamePattern = @"^@?[a-zA-Z_]\w*(\.@?[a-zA-Z_]\w*)*$";

        public static bool CheckPropertyName(string propertyName)
        {
            return new Regex(propertyNamePattern).IsMatch(propertyName);
        }

    }
}
