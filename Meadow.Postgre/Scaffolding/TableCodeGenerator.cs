using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Postgre.Scaffolding
{
   
    
    public class TableCodeGenerator : TableCodeSnippetGenerator
    {
        public TableCodeGenerator(Type type, MeadowConfiguration configuration)
            : base(new SnippetConstruction
            {
                EntityType = type,
                MeadowConfiguration = configuration
            }, SnippetConfigurations.Default())
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableCodeSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public TableCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base( construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new PostgreSqlExpressionTranslator(),
                TypeNameMapper = new PostgreDbTypeNameMapper()
            })
        {
        }

        private readonly string _keyDbQTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();


        private string GetParameters()
        {
            var parameters = "";
            var parametersTail = "";

            if (ProcessedType.HasId)
            {
                parameters = $"{ProcessedType.IdParameter.Name.DoubleQuot()} ";

                parameters += TypeCheck.IsNumerical(ProcessedType.IdField.Type)
                    ? "SERIAL"
                    : ProcessedType.IdParameter.Type;

                if (ProcessedType.NoneIdParameters.Count > 0)
                {
                    parameters += ",\n    ";
                }

                parametersTail = $"PRIMARY KEY ({ProcessedType.IdParameter.Name.DoubleQuot()})";
            }

            parameters += string.Join(",\n    ", ProcessedType.NoneIdParameters
                .Select(p => $"{p.Name.DoubleQuot()} {p.Type}"));

            parameters += ",\n    " + parametersTail;

            return parameters;
        }


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyCreationHeader, GetCreationHeader());

            replacementList.Add(_keyDbQTableName, GetTableName().DoubleQuot());

            replacementList.Add(_keyParameters, GetParameters());
        }


        private string GetTableName()
        {
             return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.TableName);
        }

        private string GetCreationHeader()
        {
            var creationHeader = "create table";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = $"drop table if exists \"{ProcessedType.NameConvention.TableName}\";" +
                                 $"\ncreate table";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = "create table if not exists";
            }

            return creationHeader;
        }

        protected override string Template => $@"
{_keyCreationHeader} {_keyDbQTableName}({_keyParameters}
);
------------------------------------------------------------------------------------------------------------------------
-- SPLIT
------------------------------------------------------------------------------------------------------------------------";
    }
}