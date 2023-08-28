using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : SqLiteByTemplateProcedureGeneratorBase
    {
        public UpdateProcedureGenerator(Type type,MeadowConfiguration configuration) : base(type,configuration)
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.UpdateProcedureName);

            replacementList.Add(_keyParameters, ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS

    UPDATE {_keyTableName} SET {_keyNoneIdParametersSet} WHERE {_keyTableName}.{_keyIdFieldName}=@{_keyIdFieldName};
GO
".Trim();
    }
}