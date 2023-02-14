using System;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Sqlable;

namespace Meadow.Scaffolding.Extensions
{
    public static  class ParameterExtensions
    {
        
        
        
        public static string ToSql(this Parameter parameter, string parameterNamePrefix)
        {
            return parameterNamePrefix + parameter.Name + " " + parameter.Type.ToUpper();
        }

        public static bool Parse(this Parameter parameter,string sql,string parameterNamePrefix)
        {
            if (string.IsNullOrWhiteSpace(parameterNamePrefix))
            {
                parameterNamePrefix = "";
            }
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

            if (!name.ToLower().StartsWith(parameterNamePrefix.ToLower()))
            {
                return false;
            }

            parameter.Name = name;

            parameter.Type = "";
            
            var sep = "";
            
            for (int i = 1; i < nameValue.Length; i++)
            {
                parameter.Type += sep + nameValue[i];

                sep = " ";
            }

            return true;
        }
    }
}