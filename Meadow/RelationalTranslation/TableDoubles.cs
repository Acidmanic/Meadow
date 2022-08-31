using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.RelationalTranslation
{
    public static class TableDoubles
    {


        public static TableDouble Simple(AccessNode node)
        {
            var table = new TableDouble
            {
                Fields = new List<string>(),
                Joins = new List<string>(),
                DataType =  node.Type
            };
            if (node.IsCollection)
            {
                table.Name = node.GetChildren()[0].Name;
            }
            else
            {
                table.Name = node.Name;
            }

            var leaves = node.GetDirectLeaves();
            
            leaves.ForEach(l => table.Fields.Add(l.Name));
            
            return table;
        }
    }
}