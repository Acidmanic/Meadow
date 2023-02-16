using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class TableScriptGenerator<TEntity> : TableScriptGenerator
    {
        public TableScriptGenerator() : base(typeof(TEntity))
        {
        }
    }

    public class TableScriptGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;

        public TableScriptGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            _type = type;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process(_type);

            replacementList.Add(_keyTableName,
                IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : process.NameConvention.TableName);

            var parameters = GetParameters(process);

            replacementList.Add(_keyParameters, parameters);
        }

        private string GetParameters(ProcessedType process)
        {
            var parameters = string.Join(',', process.NoneIdParameters.Select(p => p.Name + " " + p.Type));

            if (process.HasId)
            {
                var idParam = process.IdParameter.Name + " " + process.IdParameter.Type;

                idParam += process.IdField.IsUnique ? " NOT NULL PRIMARY KEY" : "";

                idParam += process.IdField.IsAutoValued ? " AUTO_INCREMENT" : "";

                idParam += process.NoneIdParameters.Count > 0 ? "," : "";

                parameters = idParam + parameters;
            }

            return parameters;
        }

        protected override string Template => $@"
CREATE TABLE {_keyTableName}
(
    {_keyParameters}
);
".Trim();
    }
}