using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableCodeGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public TableCodeGenerator(Type type, MeadowConfiguration configuration)
            : base(new PostgreDbTypeNameMapper(), configuration)
        {
            ProcessedType = Process(type);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();


        private string GetParameters()
        {
            var parameters = "";
            var parametersTail = "";

            if (ProcessedType.HasId)
            {
                parameters = $"{ProcessedType.IdParameter.Name.DoubleQuot()} ";

                parameters += IsNumeric(ProcessedType.IdField.Type) ? "SERIAL" : ProcessedType.IdParameter.Type;

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

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            replacementList.Add(_keyParameters, GetParameters());
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
{_keyCreationHeader} {_keyTableName}({_keyParameters}
);
------------------------------------------------------------------------------------------------------------------------
-- SPLIT
------------------------------------------------------------------------------------------------------------------------";
    }
}