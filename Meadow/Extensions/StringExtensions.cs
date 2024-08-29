using System;
using System.Text;

namespace Meadow.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64String(this string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.Default;
            
            if (value == null)
            {
                return null;
            }

            if (value.Length == 0)
            {
                return "";
            }

            var bytes = encoding.GetBytes(value);

            return Convert.ToBase64String(bytes);
        }


        public static string FromBase64String(this string base64, Encoding? encoding = null)
        {
            encoding ??= Encoding.Default;
            
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            var bytes = Convert.FromBase64String(base64);

            return encoding.GetString(bytes);
        }
    }
}