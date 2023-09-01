using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class UpdateProcedureGenerator<TEntity> : UpdateProcedureGenerator
    {
        public UpdateProcedureGenerator(MeadowConfiguration configuration) : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : MySqlProcedureGeneratorBase
    {
        public UpdateProcedureGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : Processed.NameConvention.UpdateProcedureName;
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, Processed.NameConvention.TableName);

            var parameters = string.Join(',', Processed.Parameters.Select(p => ParameterNameTypeJoint(p, "IN ")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', Processed.NoneIdParameters.Select(p => p.Name + "=" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, Processed.IdParameter.Name);
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName}({_keyParameters})
BEGIN
    UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
END;
".Trim();
    }
}