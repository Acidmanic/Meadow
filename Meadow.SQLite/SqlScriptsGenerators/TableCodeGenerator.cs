using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    
    public class TableCodeGenerator : TableSnippetGenerator
    {
        public TableCodeGenerator(Type type, MeadowConfiguration configuration) :
            base(new SnippetConstruction
            {
                EntityType = type,
                MeadowConfiguration = configuration
            }, SnippetConfigurations.Default())
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public  class TableSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdParameters = GenerateKey();
        private readonly string _keyNoneIdParameters = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();

        public TableSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(new SqLiteTypeNameMapper(), construction, configurations)
        {
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var creationHeader = GetCreationHeader();

            replacementList.Add(_keyCreationHeader, creationHeader);

            replacementList.Add(_keyTableName, GetTableName());

            var idParameters = "\n";

            if (ProcessedType.HasId)
            {
                idParameters += "    " + ParameterNameTypeJoint(ProcessedType.IdParameter);

                idParameters += " NOT NULL PRIMARY KEY";

                if (ProcessedType.IdField.IsAutoValued && TypeCheck.IsNumerical(ProcessedType.IdField.Type))
                {
                    idParameters += " AUTOINCREMENT";
                }
            }

            if (ProcessedType.NoneIdParameters.Count > 0)
            {
                idParameters += ",\n";
            }

            replacementList.Add(_keyIdParameters, idParameters);

            replacementList.Add(_keyNoneIdParameters,
                ParameterNameTypeJoint(ProcessedType.NoneIdParameters, ","));
        }

        protected virtual string GetTableName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.TableName);
        }

        private string GetCreationHeader()
        {
            var creationHeader = "CREATE TABLE";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "DROP TABLE IF EXISTS " + GetTableName() + ";" +
                                 "\nCREATE TABLE";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE TABLE IF NOT EXISTS ";
            }

            return creationHeader;
        }

        protected override string Template => $@"
{_keyCreationHeader} {_keyTableName}({_keyIdParameters}
    {_keyNoneIdParameters}
);
{LineMacro.CommentLine}
-- SPLIT
{LineMacro.CommentLine}
".Trim();
    }
}