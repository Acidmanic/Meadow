using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class InsertProcedureGenerator<TEntity> : InsertProcedureGenerator
    {
        public InsertProcedureGenerator(MeadowConfiguration configuration)
            : base(typeof(TEntity),configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertProcedureGenerator : MySqlProcedureGeneratorBase
    {
        public InsertProcedureGenerator(Type type,MeadowConfiguration configuration) : base(type,configuration)
        {
        }
        
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();


        protected override string GetProcedureName()
        {
            return IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : Processed.NameConvention.InsertProcedureName;
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = string.Join(',', Processed.NoneIdParameters
                .Select(p => "IN " + p.Name + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, Processed.NameConvention.TableName);

            var columnsAndValues = string.Join(',', Processed.NoneIdParameters
                .Select(p => p.Name));

            replacementList.Add(_keyColumns, columnsAndValues);

            replacementList.Add(_keyValues, columnsAndValues);

            replacementList.Add(_keyIdColumn, Processed.IdField.Name);
        }

        protected override string Template => @$"
{KeyCreationHeader} {KeyProcedureName}({_keyParameters})
BEGIN
    INSERT INTO {_keyTableName} ({_keyColumns}) VALUES ({_keyValues});
    SET @nid = (select LAST_INSERT_ID());
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdColumn}=@nid;
END;
".Trim();
    }
}