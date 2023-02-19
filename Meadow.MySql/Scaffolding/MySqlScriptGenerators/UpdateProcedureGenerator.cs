using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class UpdateProcedureGenerator<TEntity> : UpdateProcedureGenerator
    {
        public UpdateProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly Type _type;

        public UpdateProcedureGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            _type = type;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process(_type);

            replacementList.Add(_keyTableName, process.NameConvention.TableName);

            replacementList.Add(_keyProcedureName,
                IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : process.NameConvention.UpdateProcedureName);

            var parameters = string.Join(',', process.Parameters.Select(p => ParameterNameTypeJoint(p, "IN ")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', process.NoneIdParameters.Select(p => p.Name + "=" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, process.IdParameter.Name);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}({_keyParameters})
BEGIN
    UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
END;
".Trim();
    }
}