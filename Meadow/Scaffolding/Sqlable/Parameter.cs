using System;

namespace Meadow.Scaffolding.Sqlable
{
    public class Parameter:ISqlable,IParsable
    {
        
        public string Name { get; set; }
        public string Type { get; set; }
        
        public string ToSql()
        {
            return Name + " " + Type.ToUpper();
        }

        public bool Parse(string sql)
        {
            sql = sql.SafeTrim();

            if (string.IsNullOrEmpty(sql))
            {
                return false;
            }

            var nameValue = sql.SplitByWhiteSpaces(StringSplitOptions.RemoveEmptyEntries);

            if (nameValue.Length < 2)
            {
                return false;
            }

            var name = nameValue[0];

            if (!name.StartsWith("@"))
            {
                return false;
            }

            Name = name;

            Type = "";
            
            var sep = "";
            
            for (int i = 1; i < nameValue.Length; i++)
            {
                Type += sep + nameValue[i];

                sep = " ";
            }

            return true;
        }
    }
}