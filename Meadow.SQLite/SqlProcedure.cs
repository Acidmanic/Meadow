using System.Collections.Generic;

namespace Meadow.SQLite
{
    public class SqLiteProcedure
    {
        public string Code { get; set; }

        public List<string> ParameterNames { get; set; }

        public string Name { get; set; }

        public string GetKey()
        {
            return GetKey(Name);
        }

        public static string GetKey(string name)
        {
            return name?.ToLower();
        }
    }
}