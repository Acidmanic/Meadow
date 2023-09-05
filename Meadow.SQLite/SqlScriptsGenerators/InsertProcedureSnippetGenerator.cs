using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase
    {
        public InsertProcedureSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }


        public InsertProcedureSnippetGenerator(Type entityType, MeadowConfiguration configuration)
            : this(new SnippetConstruction
            {
                EntityType = entityType,
                MeadowConfiguration = configuration
            }, SnippetConfigurations.Default())
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            var parameters = ParameterNameTypeJoint(ProcessedType.NoneIdParameters, ",", "@");

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var columns = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name));
            var values = string.Join(',', ProcessedType.NoneIdParameters.Select(p => "@" + p.Name));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName);
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS
    INSERT INTO {_keyTableName} ({_keyColumns})
    VALUES ({_keyValues});
    SELECT * FROM {_keyTableName} WHERE ROWID=LAST_INSERT_ROWID();
GO
".Trim();
    }
}