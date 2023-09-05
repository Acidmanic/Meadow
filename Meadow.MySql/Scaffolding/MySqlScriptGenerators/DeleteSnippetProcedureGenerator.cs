using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class DeleteSnippetProcedureGenerator<TEntity> : DeleteSnippetProcedureGenerator
    {
        public DeleteSnippetProcedureGenerator(MeadowConfiguration configuration,bool allNotById)
            : base(typeof(TEntity),configuration, allNotById)
        {
        }
    }

    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteSnippetProcedureGenerator : MySqlSnippetProcedureGeneratorBase
    {
        private bool AllNotById { get; }

        public DeleteSnippetProcedureGenerator(Type type,MeadowConfiguration configuration, bool byId) 
            : base(type,configuration)
        {
            AllNotById = !byId;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldTypeName = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
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