using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class InsertSnippetProcedureGenerator<TEntity> : InsertSnippetProcedureGenerator
    {
        public InsertSnippetProcedureGenerator(MeadowConfiguration configuration)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            },SnippetConfigurations.Default())
        {
        }
    }

    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertSnippetProcedureGenerator : MySqlRepetitionHandlerProcedureGeneratorBase
    {
       
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdColumn = GenerateKey();


        public InsertSnippetProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }
        
        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = string.Join(',', ProcessedType.NoneIdParameters
                .Select(p => "IN " + p.Name + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var columnsAndValues = string.Join(',', ProcessedType.NoneIdParameters
                .Select(p => p.Name));

            replacementList.Add(_keyColumns, columnsAndValues);

            replacementList.Add(_keyValues, columnsAndValues);

            replacementList.Add(_keyIdColumn, ProcessedType.IdField.Name);
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