using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class InsertProcedureGenerator<TEntity> : ByTemplateSqlGeneratorBase
    {
        public InsertProcedureGenerator() : base(new MySqlDbTypeNameMapper())
        {
        }


        private readonly string _keyName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var processed = Process<TEntity>();
            
            replacementList.Add(_keyName,processed.NameConvention.TableName);

            var parameters = string.Join(',', processed.NoneIdParameters
                .Select(p => "IN " + p.Name + " " + p.Type));
            
            replacementList.Add(_keyParameters,parameters);
            
            replacementList.Add(_keyTableName,processed.NameConvention.TableName);

            var columnsAndValues = string.Join(',', processed.NoneIdParameters
                .Select(p => p.Name));
            
            replacementList.Add(_keyColumns,columnsAndValues);
            
            replacementList.Add(_keyValues,columnsAndValues);
            
            replacementList.Add(_keyIdColumn,processed.IdField.Name);

        }

        protected override string Template => @$"
CREATE PROCEDURE {_keyName}({_keyParameters})
BEGIN
    INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn}=last_insert_id();
END;
";
    }
}