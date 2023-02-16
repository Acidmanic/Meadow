using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.SqlScriptsGenerators
{
    public class InsertProcedureGenerator<TEntity> : InsertProcedureGenerator
    {
        public InsertProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    public class InsertProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }

        public InsertProcedureGenerator(Type type) : base(new SqlDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyRecordPhrase = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.InsertProcedureName);

            var parameters = ProcessedType.NoneIdParameters.Select(p => SqlProcedureDeclaration(p, "@"));
            var parametersClause =
                ProcessedType.NoneIdParameters.Count > 0 ? "(" + string.Join(',', parameters) + ")" : "";

            replacementList.Add(_keyParameters, parametersClause);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyColumns, string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name)));

            replacementList.Add(_keyValues,
                string.Join(',', ProcessedType.NoneIdParameters.Select(p => "@" + p.Name)));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            var recordItems = ProcessedType.Parameters.Select(p => p.Name + " @" + p.Name);

            var recordPhrase = string.Join(',', recordItems);
        }

        protected override string Template => ProcessedType.HasId ? ByIdTemplate : NoIdTemplate;

        private string ByIdTemplate => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParameters} AS
    
    INSERT INTO {_keyTableName} ({_keyColumns}) 
                   VALUES ({_keyValues})
    DECLARE @newId bigint=(IDENT_CURRENT('{_keyTableName}'));
    SELECT * FROM {_keyTableName} WHERE {_keyIdFieldName}=@newId;
GO
";

        private string NoIdTemplate => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParameters} AS
    
    INSERT INTO {_keyTableName} ({_keyColumns}) 
           VALUES ({_keyValues})
    SELECT {_keyRecordPhrase};
GO
";
    }
}