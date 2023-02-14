using System;
using System.Collections.Generic;
using Meadow.Contracts;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class DeleteProcedureGenerator<TEntity> : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }

        public DeleteProcedureGenerator(bool byId) : base(new SqLiteTypeNameMapper())
        {
            ById = byId;
        }

        protected string GetProcedureName(NameConvention nameConvention)
        {
            return ById
                ? nameConvention.DeleteByIdProcedureName
                : nameConvention.DeleteAllProcedureName;
        }

        private readonly string _keyName = GenerateKey();
        private readonly string _keyParams = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdName = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process<TEntity>();

            var procName = GetProcedureName(process.NameConvention);

            replacementList.Add(_keyName, procName);

            var idPar = "IN " + process.IdParameter.Name + " " + process.IdParameter.Type;

            replacementList.Add(_keyParams, idPar);

            replacementList.Add(_keyTableName, process.NameConvention.TableName);

            replacementList.Add(_keyIdName, process.IdField.Name);
        }

        protected override string Template => ById ? TemplateById : TemplateAll;

        private string TemplateAll => $@"
CREATE PROCEDURE {_keyName}() 
AS
    DECLARE @existing int = (SELECT COUNT(*) FROM {_keyTableName})
    DELETE FROM {_keyTableName};
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM {_keyTableName})
    IF @delta > 0 OR @existing = 0
        SELECT CAST(1 as bit) Success
    ELSE
        SELECT CAST(0 as bit) Success
GO
";

        private string TemplateById => $@"
CREATE PROCEDURE {_keyName}({_keyParams}) 
AS
    DECLARE @existing int = (SELECT COUNT(*) FROM {_keyTableName})
    DELETE FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdName} = {_keyIdName}
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM {_keyTableName})
    IF @delta > 0 OR @existing = 0
        SELECT CAST(1 as bit) Success
    ELSE
        SELECT CAST(0 as bit) Success
GO
";
    }
}