using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertProcedureGenerator : SqLiteByTemplateProcedureGeneratorBase
    {
        public InsertProcedureGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.InsertProcedureName);

            var parameters = ParameterNameTypeJoint(ProcessedType.NoneIdParameters, ",", "@");

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var columns = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name));
            var values = string.Join(',', ProcessedType.NoneIdParameters.Select(p => "@" + p.Name));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
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