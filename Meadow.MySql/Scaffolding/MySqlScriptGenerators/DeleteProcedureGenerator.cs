using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class DeleteProcedureGenerator<TEntity> : DeleteProcedureGenerator
    {
        public DeleteProcedureGenerator(bool allNotById) : base(typeof(TEntity),allNotById)
        {
        }
    }
    
    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private bool AllNotById { get; }

        private readonly Type _type;

        public DeleteProcedureGenerator(Type type, bool byId) : base(new MySqlDbTypeNameMapper())
        {
            AllNotById = !byId;
            _type = type;
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldTypeName = GenerateKey();


        private string GetProcedureName(ProcessedType processed)
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return AllNotById
                ? processed.NameConvention.DeleteAllProcedureName
                : processed.NameConvention.DeleteByIdProcedureName;
        }
        
        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var processed = Process(_type);
            
            replacementList.Add(_keyProcedureName,GetProcedureName(processed));
            
            replacementList.Add(_keyTableName,processed.NameConvention.TableName);
            
            replacementList.Add(_keyIdFieldName, processed.HasId?processed.IdParameter.Name:"[NO-ID-FIELD]");
            
            replacementList.Add(_keyIdFieldTypeName, processed.HasId?processed.IdParameter.Type:"[NO-ID-FIELD]");
            
            
        }


        private string TemplateAll => $@"
CREATE PROCEDURE {_keyProcedureName}() 
BEGIN
    DELETE FROM {_keyTableName};
    SELECT TRUE Success;
END;
".Trim();

        private string TemplateById => $@"
CREATE PROCEDURE {_keyProcedureName}(IN {_keyIdFieldName} {_keyIdFieldTypeName}) 
BEGIN
    DELETE FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName};
    SELECT TRUE Success;
END;
".Trim();

        protected override string Template => AllNotById ? TemplateAll : TemplateById;
    }
}