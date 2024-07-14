using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class TableScriptSnippetGenerator<TEntity> : TableScriptSnippetGenerator
    {
        public TableScriptSnippetGenerator(MeadowConfiguration configuration)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            }, SnippetConfigurations.Default())
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public TableScriptSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new MySqlExpressionTranslator(),
                TypeNameMapper = new MySqlDbTypeNameMapper()
            })
        {
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyDropping = GenerateKey();
        private readonly string _keyCreation = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var dropping = "";
            var creation = "CREATE TABLE";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                dropping = "DROP TABLE IF EXISTS " + ProcessedType.NameConvention.TableName + ";\n";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creation = "CREATE TABLE IF NOT EXISTS";
            }

            replacementList.Add(_keyDropping, dropping);
            replacementList.Add(_keyCreation, creation);

            replacementList.Add(_keyTableName, GetTableName(ProcessedType));

            var parameters = GetParameters(ProcessedType);

            replacementList.Add(_keyParameters, parameters);
        }


        private string GetTableName(ProcessedType process)
        {
            if (Configurations.OverrideDbObjectName)
            {
                return Configurations.OverrideDbObjectName.Value(Construction);
            }

            return process.NameConvention.TableName;
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
{_keyDropping}{_keyCreation} {_keyTableName}
(
    {_keyParameters}
);
".Trim();
    }
}