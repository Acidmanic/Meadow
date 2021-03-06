using System;
using Meadow.DataTypeMapping;
using Meadow.Reflection;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class TableScriptGenerator : SqlGeneratorBase
    {
        public override DbObjectTypes ObjectType => DbObjectTypes.Tables;

        public TableScriptGenerator(Type type) : base(type)
        {
        }

        public override Code Generate(SqlScriptActions action)
        {
            var sep = "";
            var parameters = "";

            WalkThroughLeaves(false, leaf =>
            {
                string fieldName = leaf.Name;

                string typeName = TypeNameMapper[leaf.Type];

                string id = leaf.IsUnique ? " IDENTITY (1,1) NOT NULL PRIMARY KEY" : "";

                parameters += sep + fieldName + " " + typeName + id;

                sep = ", ";
            });

            var tableScript = "";
            var createKeyword = "CREATE";
            if (action == SqlScriptActions.DropCreate)
            {
                tableScript = $"DROP TABLE {NameConvention.TableName}\n\n";
            }
            else if (action == SqlScriptActions.Alter)
            {
                createKeyword = "ALTER";
            }

            tableScript = $"{createKeyword} TABLE {NameConvention.TableName} (\n\t{parameters}\n\t)\n\n";

            return new Code
            {
                Name = NameConvention.TableName,
                Text = tableScript
            };
        }

        public override string SqlObjectName => NameConvention.TableName;
    }
}