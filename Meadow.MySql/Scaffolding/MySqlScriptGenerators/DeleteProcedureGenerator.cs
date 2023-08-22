using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class DeleteProcedureGenerator<TEntity> : DeleteProcedureGenerator
    {
        public DeleteProcedureGenerator(bool allNotById) : base(typeof(TEntity), allNotById)
        {
        }
    }

    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteProcedureGenerator : MySqlProcedureGeneratorBase
    {
        private bool AllNotById { get; }

        public DeleteProcedureGenerator(Type type, bool byId) : base(type)
        {
            AllNotById = !byId;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldTypeName = GenerateKey();


        protected override string GetProcedureName()
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return AllNotById
                ? Processed.NameConvention.DeleteAllProcedureName
                : Processed.NameConvention.DeleteByIdProcedureName;
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, Processed.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName, Processed.HasId ? Processed.IdParameter.Name : "[NO-ID-FIELD]");

            replacementList.Add(_keyIdFieldTypeName, Processed.HasId ? Processed.IdParameter.Type : "[NO-ID-FIELD]");
        }


        private string TemplateAll => $@"
{KeyCreationHeader} {KeyProcedureName}() 
BEGIN
    DELETE FROM {_keyTableName};
    SELECT TRUE Success;
END;
".Trim();

        private string TemplateById => $@"
{KeyCreationHeader} {KeyProcedureName}(IN {_keyIdFieldName} {_keyIdFieldTypeName}) 
BEGIN
    DELETE FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
    SELECT TRUE Success;
END;
".Trim();

        protected override string Template => AllNotById ? TemplateAll : TemplateById;
    }
}