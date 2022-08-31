using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.RelationalTranslation
{
    public class Select
    {
        public string Name { get; set; }

        public List<string> OriginalJoins { get; set; }

        public List<string> OriginalFields { get; set; }

        public string Phrase { get; set; }


        public Select()
        {
            OriginalFields = new List<string>();
            OriginalJoins = new List<string>();

        }

        private Select(TableDouble table)
        {
            OriginalFields = new List<string>();
            OriginalJoins = new List<string>();
            Name = table.Name;
            OriginalFields.AddRange(table.Fields);
            OriginalJoins.AddRange(table.Joins);
        }

        public static Select From(TableDouble table)
        {
            var select = new Select(table)
            {
                Phrase = "FROM " + table.Name,
            };
            return select;
        }
        

        public static Select Join(TableDouble main, TableDouble table, char delimiter,bool forwards)
        {
            string junction = "";
            
            if (forwards)
            {
                junction = GetJunction(main, table, delimiter);
            }else
            {
                junction = GetJunction( table,main, delimiter);
            }
            
            var select = new Select(table)
            {
                Phrase = $"LEFT JOIN {table.Name} ON {junction}"
            };
            return select;
            
        }
        private static string GetJunction(TableDouble main, TableDouble table,char delimiter)
        {
            var childIdLeaf = IdHelper.GetIdLeaf(table.DataType);

            if (childIdLeaf == null)
            {
                return "{UNABLE_TO_DETERMINE_JUNCTION}";
            }

            var joinee = table.Name + "." + childIdLeaf.Name;

            var joiner = main.Name + "." + table.Name + childIdLeaf.Name;

            var equality = GetEquality(childIdLeaf.Type);

            return joiner + " " + equality + " " + joinee;
        }

        private static string GetEquality(Type type)
        {
            if (type == typeof(string))
            {
                return "like";
            }

            return "=";
        }
    }
}
