using System;
using Meadow.Scaffolding;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class TableScriptGenerator : SqlGeneratorBase
    {
        public override DbObjectTypes ObjectType => DbObjectTypes.Tables;

        public TableScriptGenerator(Type type) : base(type)
        {
            UseDbTypeMapper(new MySqlDbTypeNameMapper());
        }

        public override Code Generate(SqlScriptActions action)
        {
            var sep = "";
            var parameters = "";

            WalkThroughLeaves(false, leaf =>
            {
                string fieldName = leaf.Name;

                string typeName = TypeNameMapper[leaf.Type];

                var id = "";

                id += leaf.IsUnique ? " NOT NULL PRIMARY KEY" : "";

                id += leaf.IsAutoValued ? " auto_increment":"";

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