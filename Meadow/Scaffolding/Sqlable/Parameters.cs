using System;
using System.Collections.Generic;

namespace Meadow.Scaffolding.Sqlable
{
    public class Parameters : List<Parameter>, ISqlable,IParsable
    {
        public string ToSql()
        {
            if (this.Count == 0)
            {
                return "";
            }

            string sql = "(";

            string sep = "";
            foreach (var parameter in this)
            {
                sql += sep + parameter.ToSql();
                sep = ",";
            }

            sql += ")";

            return sql;
        }

        public bool Parse(string sql)
        {
            sql = sql.SafeTrim();
            
            if (string.IsNullOrEmpty(sql))
            {
                this.Clear();
                return true;
            }

            if (!sql.StartsWith("(") && sql.EndsWith(")"))
            {
                return false;
            }

            sql = sql.Substring(1, sql.Length - 2);

            var paramSqls = sql.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (var paramSql in paramSqls)
            {
                var parameter = new Parameter();

                if (!parameter.Parse(paramSql))
                {
                    return false;
                }
                this.Add(parameter);
            }

            return true;
        }
    }
}