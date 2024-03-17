using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class DeleteSnippetProcedureGenerator<TEntity> : DeleteSnippetProcedureGenerator
    {
        public DeleteSnippetProcedureGenerator(MeadowConfiguration configuration, bool allNotById)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            }, SnippetConfigurations.IdAware(allNotById))
        {
        }
    }

    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteSnippetProcedureGenerator : MySqlRepetitionHandlerProcedureGeneratorBase, IIdAware
    {
        protected override string Template => ActById ? TemplateAll : TemplateById;

        public DeleteSnippetProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }

        public bool ActById { get; set; }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldTypeName = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ActById
                ? ProcessedType.NameConvention.DeleteAllProcedureName
                : ProcessedType.NameConvention.DeleteByIdProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            if (!ProcessedType.HasId)
            {
                return;
            }
            
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");

            replacementList.Add(_keyIdFieldTypeName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Type : "[NO-ID-FIELD]");
        }


        private string TemplateAll => ProcessedType.HasId? $@"
{KeyCreationHeader} {KeyProcedureName}() 
BEGIN
    DELETE FROM {_keyTableName};
    SELECT TRUE Success;
END;
".Trim():"";

        private string TemplateById => $@"
{KeyCreationHeader} {KeyProcedureName}(IN {_keyIdFieldName} {_keyIdFieldTypeName}) 
BEGIN
    DELETE FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
    SELECT TRUE Success;
END;
".Trim();
    }
}