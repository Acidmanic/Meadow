using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class InsertProcedureGenerator<TEntity> : InsertProcedureGenerator
    {
        public InsertProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    public class InsertProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;

        public InsertProcedureGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            _type = type;
        }


        private readonly string _keyName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var processed = Process(_type);

            replacementList.Add(_keyName,
                IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : processed.NameConvention.InsertProcedureName);

            var parameters = string.Join(',', processed.NoneIdParameters
                .Select(p => "IN " + p.Name + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, processed.NameConvention.TableName);

            var columnsAndValues = string.Join(',', processed.NoneIdParameters
                .Select(p => p.Name));

            replacementList.Add(_keyColumns, columnsAndValues);

            replacementList.Add(_keyValues, columnsAndValues);

            replacementList.Add(_keyIdColumn, processed.IdField.Name);
        }

        protected override string Template => @$"
CREATE PROCEDURE {_keyName}({_keyParameters})
BEGIN
    INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn}=LAST_INSERT_ID();
END;
".Trim();
    }
}