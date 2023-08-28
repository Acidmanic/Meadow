using System;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Sql;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type) : base(type, new MySqlDbTypeNameMapper())
        {
        }
    }
}