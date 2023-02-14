using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class TableScriptGenerator<TEntity> : ByTemplateSqlGeneratorBase
    {
        public TableScriptGenerator() : base(new MySqlDbTypeNameMapper())
        {
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process<TEntity>();

            replacementList.Add(_keyTableName, process.NameConvention.TableName);

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

                idParam += process.IdField.IsAutoValued ? " auto_increment" : "";

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
";
    }
}