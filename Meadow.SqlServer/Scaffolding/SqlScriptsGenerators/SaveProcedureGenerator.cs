using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class SaveProcedureGenerator<TEntity> : SaveProcedureGenerator
    {
        public SaveProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;

        public SaveProcedureGenerator(Type type) : base(new SqlDbTypeNameMapper())
        {
            _type = type;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();
        
        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var processed = Process(_type);

            replacementList.Add(_keyTableName, processed.NameConvention.TableName);

            replacementList.Add(_keyProcedureName,IsDatabaseObjectNameForced?ForcedDatabaseObjectName: processed.NameConvention.SaveProcedureName);

            var parameters = string.Join(',', processed.Parameters.Select(p => SqlProcedureDeclaration(p, "@")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', processed.NoneIdParameters.Select(p => p.Name + "= @" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, processed.IdParameter.Name);
            replacementList.Add(_keyIdFieldType, processed.IdParameter.Type);
            
            replacementList.Add(_keyWhereClause,GetWhereClause(processed));
            
            var columns = string.Join(',', processed.NoneIdParameters
                .Select(p => p.Name));
            
            var values = string.Join(',', processed.NoneIdParameters
                .Select(p => "@"+p.Name));
            
            replacementList.Add(_keyColumns,columns);
            
            replacementList.Add(_keyValues,values);
            
            replacementList.Add(_keyIdColumn,processed.IdField.Name);
        }

        private bool IsString(Parameter p)
        {
            var typeLower = p.Type.ToLower().Trim();

            return typeLower.StartsWith("text") ||
                   typeLower.StartsWith("varchar") ||
                   typeLower.StartsWith("nvarchar");
        }

        private string EqualityAssertion(Parameter p)
        {
            return IsString(p) ? "like" : "=";
        }

        private string EqualityClause(string tableName, Parameter p)
        {
            return tableName + "." + p.Name + " " + EqualityAssertion(p) + " @" + p.Name;
        }

        private string GetWhereClause(ProcessedType process)
        {
            if (process.NoneIdUniqueParameters.Count > 0)
            {
                return string.Join("AND ", process.NoneIdUniqueParameters.Select(p =>
                    EqualityClause(process.NameConvention.TableName, p)));
            }

            return EqualityClause(process.NameConvention.TableName, process.IdParameter);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}({_keyParameters}) AS
    IF EXISTS(SELECT 1 FROM {_keyTableName} WHERE {_keyWhereClause})
        BEGIN
            UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyWhereClause};
        
            SELECT TOP 1 * FROM {_keyTableName} WHERE {_keyWhereClause} ORDER BY {_keyIdColumn} ASC;
        END
    ELSE
        BEGIN
            INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});

            DECLARE @newId {_keyIdFieldType}=(IDENT_CURRENT('{_keyTableName}'));

            SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn} = @newId;
        END
GO
".Trim();
    }
}