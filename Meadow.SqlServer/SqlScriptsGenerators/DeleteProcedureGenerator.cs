using System;
using System.Collections.Generic;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.SqlScriptsGenerators
{


    public class DeleteProcedureGenerator<TEntity> : DeleteProcedureGenerator
    {
        public DeleteProcedureGenerator(bool byId) : base(typeof(TEntity), byId)
        {
        }
    }
    
    public class DeleteProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }

        private readonly Type _type;
        
        private ProcessedType ProcessedType { get; }

        public DeleteProcedureGenerator(Type type, bool byId) : base(new SqlDbTypeNameMapper())
        {
            _type = type;
            ById = byId;
            ProcessedType = Process(_type);
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParametersParentheses = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        
        
        protected string GetProcedureName()
        {
            return ById
                ? ProcessedType.NameConvention.DeleteByIdProcedureName
                : ProcessedType.NameConvention.DeleteAllProcedureName;
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName,GetProcedureName());

            var idParameter = "( @" + ProcessedType.IdParameter.Name + " "
                              + ProcessedType.IdParameter.Type + ")";
            
            replacementList.Add(_keyParametersParentheses,ById?idParameter:"");
            
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
            
            var whereClause = ById ? $"WHERE {ProcessedType.IdParameter.Name}=@{ProcessedType.IdParameter.Name}" : "";
            
            replacementList.Add(_keyWhereClause,whereClause);
            
            
        }
        
        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParametersParentheses} AS
    DECLARE @existing int = (SELECT COUNT(*) FROM {_keyTableName});
    DELETE FROM {_keyTableName} {_keyWhereClause}
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM {_keyTableName});
    IF @delta > 0 or @existing = 0
        SELECT cast(1 as bit) Success
    ELSE
        select cast(0 as bit) Success
GO
";
    }
}