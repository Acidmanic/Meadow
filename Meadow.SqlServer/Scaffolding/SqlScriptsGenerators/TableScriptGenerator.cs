using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class TableScriptGenerator<TEntity> : TableScriptGenerator
    {
        public TableScriptGenerator(bool appendSplit) : base(typeof(TEntity), appendSplit)
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class CrudTableScriptGenerator : TableScriptGenerator
    {
        public CrudTableScriptGenerator(Type type) : base(type, true)
        {
        }
    }


    public class TableScriptGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;
        private readonly bool _appendSplitter;

        public TableScriptGenerator(Type type, bool appendSplitter) : base(new SqlDbTypeNameMapper())
        {
            _type = type;
            _appendSplitter = appendSplitter;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySplitTail = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process(_type);

            replacementList.Add(_keyTableName,
                IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : process.NameConvention.TableName);

            var parameters = GetParameters(process);

            replacementList.Add(_keyParameters, parameters);

            var line = new LineMacro().GenerateCode();

            var splitTail = _appendSplitter ? ("\n" + line + "\n-- SPLIT\n" + line + "\n") : "";

            replacementList.Add(_keySplitTail, splitTail);
        }

        private string GetParameters(ProcessedType process)
        {
            var parameters = string.Join(',', process.NoneIdParameters.Select(p => p.Name + " " + p.Type));

            if (process.HasId)
            {
                var idParam = process.IdParameter.Name + " " + process.IdParameter.Type;

                idParam += process.IdField.IsUnique ? " NOT NULL PRIMARY KEY" : "";

                idParam += process.IdField.IsAutoValued ? " IDENTITY(1,1)" : "";

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
{_keySplitTail}
".Trim();
    }
}