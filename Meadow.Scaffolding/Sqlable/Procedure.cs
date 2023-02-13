using System;
using System.Collections.Generic;
using System.Security;

namespace Meadow.Scaffolding.Sqlable
{
    public class Procedure : ISqlable,IParsable
    {
        private static readonly string RE_Procedure = "\\sPROCEDURE\\s";
        private static readonly string RE_AS = "(\\s||\\))AS\\s";
        private static readonly string RE_GO = "\\sGO";
        
        //<Creation><Procedure><ProcedureName>[<Parameters>]<As><Code><Go>

        public DbObjectCreation Creation { get; set; } = DbObjectCreation.Create;

        public Parameters Parameters { get; set; } = new Parameters();
        
        public string Name { get; set; }

        public string Code { get; set; }

        public string ToSql()
        {
            return Creation.ToSql() + " " + "PROCEDURE" + " "
                   + Name + " " 
                   + Parameters.ToSql() + "\nAS\n" +
                   Code + "\nGO\n";
            ;
        }

        public bool Parse(string sql)
        {
            sql = sql.SafeTrim();

            var subSql = sql.SubString(0, RE_Procedure,true);

            if (subSql == null)
            {
                return false;
            }

            var creation = DbObjectCreation.Create;

            if (!creation.Parse(subSql))
            {
                return false;
            }

            //<Name>[<Parameters>]
            subSql = sql.SubStringBetween(RE_Procedure, RE_AS,true).SafeTrim();

            if (string.IsNullOrEmpty(subSql))
            {
                return false;
            }

            var parts = subSql.Split(new char[] {' ','\n','\r','\t','(',')' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
            {
                return false;
            }

            var name = parts[0].Trim();
            
            subSql = sql.SubStringBetween(name, RE_AS,true).SafeTrim().SafeTrim();

            subSql = subSql.SafeTrim();
            
            var parameters = new Parameters();

            if (!parameters.Parse(subSql))
            {
                return false;
            }

            if (sql.ToLower().EndsWith("go"))
            {
                Code = sql.SubStringBetween(RE_AS, RE_GO,true).SafeTrim();
            }
            else
            {
                Code = sql.SubStringAfterTag(RE_AS,true).SafeTrim();
            }

            Name = name;
            Creation = creation;
            Parameters = parameters;
            return true;

        }

        
    }
}