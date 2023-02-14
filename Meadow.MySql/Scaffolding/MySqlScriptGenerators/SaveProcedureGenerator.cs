using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class SaveProcedureGenerator<TEntity> : SaveProcedureGenerator
    {
        public SaveProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    public class SaveProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;

        public SaveProcedureGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            _type = type;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();
        
        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var processed = Process(_type);

            replacementList.Add(_keyTableName, processed.NameConvention.TableName);

            replacementList.Add(_keyProcedureName, processed.NameConvention.SaveProcedureName);

            var whereClause = GetWhereClause(processed);

            var parameters = string.Join(',', processed.Parameters.Select(p => SqlProcedureDeclaration(p, "IN ")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', processed.NoneIdParameters.Select(p => p.Name + "=" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, processed.IdParameter.Name);
            
            replacementList.Add(_keyWhereClause,GetWhereClause(processed));
            
            var columnsAndValues = string.Join(',', processed.NoneIdParameters
                .Select(p => p.Name));
            
            replacementList.Add(_keyColumns,columnsAndValues);
            
            replacementList.Add(_keyValues,columnsAndValues);
            
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
            return tableName + "." + p.Name + " " + EqualityAssertion(p) + " " + p.Name;
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
CREATE PROCEDURE {_keyProcedureName}({_keyParameters})
BEGIN
    IF EXISTS(SELECT 1 FROM {_keyTableName} WHERE {_keyWhereClause}) then
        
        UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyWhereClause};
        
        SELECT * FROM {_keyTableName} WHERE {_keyWhereClause} ORDER BY {_keyIdColumn} ASC LIMIT 1;
        
    ELSE
        INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});
        SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn} = LAST_INSERT_ID();
    END IF;
END;
".Trim();
    }
}