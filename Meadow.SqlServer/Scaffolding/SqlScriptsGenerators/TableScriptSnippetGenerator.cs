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
    public class TableScriptSnippetGenerator<TEntity> : CreateTableScriptSnippetGeneratorBase
    {
        public TableScriptSnippetGenerator(MeadowConfiguration configuration,bool appendSplitter)
            : base(new SnippetConstruction
                    { EntityType = typeof(TEntity),MeadowConfiguration = configuration}, 
                SnippetConfigurations.Default(), appendSplitter)
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptSnippetGenerator : CreateTableScriptSnippetGeneratorBase
    {
        public TableScriptSnippetGenerator(Type type, MeadowConfiguration configuration,bool appendSplitter)
            : base(new SnippetConstruction
            { EntityType = type,MeadowConfiguration = configuration}, 
                SnippetConfigurations.Default(), appendSplitter)
        {
        }

        public TableScriptSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) 
            : base(construction, configurations, true)
        {
        }
    }


    public abstract class CreateTableScriptSnippetGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        private readonly bool _appendSplitter;

        protected CreateTableScriptSnippetGeneratorBase(
            SnippetConstruction construction, 
            SnippetConfigurations configurations,
            bool appendSplitter) 
            : base(new SqlDbTypeNameMapper(), construction, configurations)
        {
            _appendSplitter = appendSplitter;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySplitTail = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {

            var creationHeader = GetCreationHeader(ProcessedType);

            replacementList.Add(_keyCreationHeader, creationHeader);

            replacementList.Add(_keyTableName,GetTableName());

            var parameters = GetParameters(ProcessedType);

            replacementList.Add(_keyParameters, parameters);

            var line = new LineMacro().GenerateCode();

            var splitTail = _appendSplitter ? ("\n" + line + "\n-- SPLIT\n" + line + "\n") : "";

            replacementList.Add(_keySplitTail, splitTail);
        }

        protected string GetTableName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.TableName);
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