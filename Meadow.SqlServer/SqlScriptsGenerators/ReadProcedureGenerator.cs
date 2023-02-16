using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.SqlScriptsGenerators
{
    public class ReadProcedureGenerator<TEntity> : ReadProcedureGenerator
    {
        public ReadProcedureGenerator(bool byId) : base(typeof(TEntity), byId)
        {
        }
    }

    public class ReadProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }
        private ProcessedType ProcessedType { get; }

        public ReadProcedureGenerator(Type type, bool byId) : base(new SqlDbTypeNameMapper())
        {
            ById = byId;
            ProcessedType = Process(type);
        }

        private string GetProcedureName()
        {
            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureName
                : ProcessedType.NameConvention.SelectAllProcedureName;
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyIdParameterDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            var parameterDeclaration =
                ById ? $"(@{ProcessedType.IdParameter.Name} {ProcessedType.IdParameter.Type})" : "";

            replacementList.Add(_keyIdParameterDeclaration, parameterDeclaration);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause = ById
                ? $" WHERE {ProcessedType.NameConvention.TableName}.{ProcessedType.IdParameter.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO
";
    }
}