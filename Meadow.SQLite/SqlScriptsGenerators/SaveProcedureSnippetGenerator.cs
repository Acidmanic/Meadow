using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveProcedureSnippetGenerator : SqLiteByTemplateProcedureSnippetGeneratorBase
    {
        public SaveProcedureSnippetGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();
        private readonly string _keyNonIdColumns = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyNoneIdParameterValues = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.SaveProcedureName);

            replacementList.Add(_keyParameters,
                ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));

            replacementList.Add(_keyNonIdColumns,
                string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name)));

            var whereClause = ParameterNameValueSetJoint(ProcessedType.NoneIdUniqueParameters, " AND ", "@");

            if (ProcessedType.NoneIdUniqueParameters.Count == 0)
            {
                whereClause = ParameterNameValueSetJoint(ProcessedType.IdParameter, "@");
            }

            replacementList.Add(_keyWhereClause, whereClause);

            replacementList.Add(_keyNoneIdParameterValues,
                string.Join(',', ProcessedType.NoneIdParameters.Select(p => "@" + p.Name)));
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS
    UPDATE {_keyTableName}  SET {_keyNoneIdParametersSet} WHERE {_keyWhereClause};
    INSERT INTO {_keyTableName} ({_keyNonIdColumns}) SELECT {_keyNoneIdParameterValues}
        WHERE NOT EXISTS(SELECT * FROM {_keyTableName} WHERE {_keyWhereClause});
    SELECT * FROM {_keyTableName} WHERE {_keyWhereClause} OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
GO
".Trim();
    }
}