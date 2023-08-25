using System;

namespace Meadow.SQLite.ProcedureProcessing
{
    public class Procedure : ISqlable, IParsable
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
            sql = sql.RemoveSqLiteComments();
            
            sql = sql.SafeTrim();

            var subSql = sql.SubString(0, RE_Procedure, true);

            if (subSql == null)
            {
                return false;
            }

            var creation = new DbObjectCreation("");

            if (!creation.Parse(subSql))
            {
                return false;
            }

            var isDrop = creation.ToSql().StartsWith("DROP");


            if (isDrop)
            {
                //<Name>GO
                subSql = sql.SubStringBetweenOrToTheEnd(RE_Procedure, RE_GO,
                    "go", true).SafeTrim();

                if (string.IsNullOrEmpty(subSql))
                {
                    return false;
                }

                Name = subSql;
                Code = "";
                Creation = creation;
                Parameters = new Parameters();
                return true;
            }

            //<Name>[<Parameters>]
            subSql = sql.SubStringBetween(RE_Procedure, RE_AS, true).SafeTrim();

            if (string.IsNullOrEmpty(subSql))
            {
                return false;
            }

            var parts = subSql.Split(new char[] { ' ', '\n', '\r', '\t', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
            {
                return false;
            }

            var name = parts[0].Trim();

            subSql = sql.SubStringBetween(name, RE_AS, true).SafeTrim().SafeTrim();

            subSql = subSql.SafeTrim();

            var parameters = new Parameters();

            if (!parameters.Parse(subSql))
            {
                return false;
            }

            Code = sql.SubStringBetweenOrToTheEnd(RE_AS, RE_GO, "go", true).SafeTrim();


            Name = name;
            Creation = creation;
            Parameters = parameters;
            return true;
        }
    }
}