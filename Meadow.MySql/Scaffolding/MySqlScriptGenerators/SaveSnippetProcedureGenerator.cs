using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveSnippetProcedureGenerator : MySqlRepetitionHandlerProcedureGeneratorBase
    {
        public SaveSnippetProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.SaveProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var parameters = string.Join(',', ProcessedType.Parameters.Select(p => ParameterNameTypeJoint(p, "IN ")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name + "=" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            replacementList.Add(_keyWhereClause, GetWhereClause(ProcessedType));

            var columnsAndValues = string.Join(',', ProcessedType.NoneIdParameters
                .Select(p => p.Name));

            replacementList.Add(_keyColumns, columnsAndValues);

            replacementList.Add(_keyValues, columnsAndValues);

            replacementList.Add(_keyIdColumn, ProcessedType.IdField.Name);
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
{KeyCreationHeader} {KeyProcedureName}({_keyParameters})
BEGIN
    IF EXISTS(SELECT 1 FROM {_keyTableName} WHERE {_keyWhereClause}) then
        
        UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyWhereClause};
        
        SELECT * FROM {_keyTableName} WHERE {_keyWhereClause} ORDER BY {_keyIdColumn} ASC LIMIT 1;
        
    ELSE
        INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});
        SET @nid = (select LAST_INSERT_ID());
        SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn} = @nid;
    END IF;
END;
".Trim();


    }
}