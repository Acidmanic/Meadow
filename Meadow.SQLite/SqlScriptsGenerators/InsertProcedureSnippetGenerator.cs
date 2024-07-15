using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
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
        private readonly string _keyEntityFilterSegment = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var insertParameters = ProcessedType.GetInsertParameters();
            
            replacementList.Add(_keyProcedureName, GetProcedureName());

            var parameters = ParameterNameTypeJoint(insertParameters, ",", "@");

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var columns = string.Join(',', insertParameters.Select(p => p.Name));
            var values = string.Join(',', insertParameters.Select(p => "@" + p.Name));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
            
            var entityFilterExpression = GetFiltersWhereClause(false);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND {entityFilterExpression.Value} " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName);
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS
    INSERT INTO {_keyTableName} ({_keyColumns})
    VALUES ({_keyValues});
    SELECT * FROM {_keyTableName} WHERE ROWID=LAST_INSERT_ROWID(){_keyEntityFilterSegment};
GO
".Trim();
    }
}