using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class TableScriptGenerator<TEntity> : CreateTableScriptGeneratorBase
    {
        public TableScriptGenerator(MeadowConfiguration configuration, bool appendSplit)
            : base(typeof(TEntity), configuration, appendSplit)
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptGenerator : CreateTableScriptGeneratorBase
    {
        public TableScriptGenerator(Type type, MeadowConfiguration configuration,bool appendSplitter)
            : base(type, configuration, appendSplitter)
        {
        }
        
        public TableScriptGenerator(Type type, MeadowConfiguration configuration)
            : base(type, configuration, true)
        {
        }
    }


    public abstract class CreateTableScriptGeneratorBase : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;
        private readonly bool _appendSplitter;

        public CreateTableScriptGeneratorBase(Type type, MeadowConfiguration configuration, bool appendSplitter) :
            base(new SqlDbTypeNameMapper(), configuration)
        {
            _type = type;
            _appendSplitter = appendSplitter;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySplitTail = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process(_type);

            var creationHeader = GetCreationHeader(process);

            replacementList.Add(_keyCreationHeader, creationHeader);

            replacementList.Add(_keyTableName,GetTableName(process));

            var parameters = GetParameters(process);

            replacementList.Add(_keyParameters, parameters);

            var line = new LineMacro().GenerateCode();

            var splitTail = _appendSplitter ? ("\n" + line + "\n-- SPLIT\n" + line + "\n") : "";

            replacementList.Add(_keySplitTail, splitTail);
        }

        protected virtual string GetTableName(ProcessedType process)
        {
            return IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : process.NameConvention.TableName;
        }

        private string GetCreationHeader(ProcessedType process)
        {
            var creationHeader = "CREATE TABLE";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "DROP TABLE IF EXISTS " + process.NameConvention.TableName +
                                 "\nCREATE TABLE";
            }
            else if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = $"IF OBJECT_ID(N'{process.NameConvention.TableName}', N'U') IS NULL" +
                                 "\nCREATE TABLE";
            }

            return creationHeader;
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
{_keyCreationHeader} {_keyTableName}
(
    {_keyParameters}
);
{_keySplitTail}
".Trim();
    }
}