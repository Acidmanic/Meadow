using System;

namespace Meadow.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64String(this string value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.Length == 0)
            {
                return "";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(bytes);
        }


        public static string FromBase64String(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            var bytes = Convert.FromBase64String(base64);

            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}