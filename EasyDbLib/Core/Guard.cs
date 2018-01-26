using System;

namespace EasyDbLib
{
    public class Guard
    {
        public static void IsNull(object value, string message = null)
        {
            if (value == null)
            {
                if (message != null)
                {
                    throw new Exception(message);
                }
                else
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }
        }

        public static void IsNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
        }

        public static void Throw(string message)
        {
            throw new Exception(message);
        }

        public static void Throw(Exception ex)
        {
            throw ex;
        }
    }
}
