namespace Meadow.Postgre
{
    public static class StringExtensions
    {


        public static string DoubleQuot(this string value)
        {
            return "\"" + value + "\"";
        }
    }
}