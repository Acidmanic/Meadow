using System;
using System.Collections.Generic;

namespace Meadow.SQLite.ProcedureProcessing
{
    public class SqLiteProcedure
    {
        public string Code { get; set; }

        
        public DbObjectCreation Creation { get; set; } = DbObjectCreation.Create;
        
        public Dictionary<string, string> ParameterTypesByParameterName { get; set; }

        public string Name { get; set; }

        public string GetKey()
        {
            return GetKey(Name);
        }

        public static string GetKey(string name)
        {
            return name?.ToLower();
        }

        private static int IndexOf(string value, params string[] find)
        {
            return IndexOf(value, 0, find);
        }


        public SqLiteProcedure()
        {
            ParameterTypesByParameterName = new Dictionary<string, string>();
        }

        private static int IndexOf(string value, int startIndex, params string[] find)
        {
            foreach (string f in find)
            {
                var index = value.IndexOf(f, startIndex, StringComparison.Ordinal);

                if (index != -1)
                {
                    return index;
                }
            }

            return -1;
        }


        public static SqLiteProcedure From(Procedure procedure)
        {
            SqLiteProcedure converted = new SqLiteProcedure();

            converted.Code = procedure.Code;

            converted.Name = procedure.Name;

            converted.Creation = procedure.Creation;

            procedure.Parameters.ForEach(pr => converted.ParameterTypesByParameterName.Add(pr.Name, pr.Type));

            return converted;
        }
        public static SqLiteProcedure Parse(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }
            code = code.Trim();

            code = NormalizeWhiteSpaces(code) + "\n";

            Procedure p = new Procedure();

            if (p.Parse(code))
            {
                var parsed = From(p);
                
                return parsed;
            }

            return null;
        }

        private static string NormalizeWhiteSpaces(string code)
        {
            string normalizedCode = "";

            var lastSpace = true;
            var lastEnter = true;

            foreach (char c in code)
            {
                if (Char.IsWhiteSpace(c))
                {
                    var enter = (c == '\n' || c == '\r');
                    var space = !enter;

                    if ((!lastEnter && !lastSpace) || (lastEnter != enter && lastSpace != space))
                    {
                        normalizedCode += c;
                    }

                    lastEnter = enter;
                    lastSpace = space;
                }
                else
                {
                    normalizedCode += c;
                    lastEnter = false;
                    lastSpace = false;
                }
            }

            return normalizedCode;
        }
    }
}