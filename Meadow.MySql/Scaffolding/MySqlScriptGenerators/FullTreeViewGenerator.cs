using System;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Sql;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type, MeadowConfiguration configuration)
            : base(type, configuration, new MySqlDbTypeNameMapper())
        {
        }
    }
}