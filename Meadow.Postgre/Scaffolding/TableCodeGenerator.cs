using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableCodeGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }

        public TableCodeGenerator(Type type) : base(new PostgreDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();


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
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            replacementList.Add(_keyParameters, GetParameters());
        }

        protected override string Template => $@"
create table {_keyTableName}({_keyParameters}
);
------------------------------------------------------------------------------------------------------------------------
-- SPLIT
------------------------------------------------------------------------------------------------------------------------";
    }
}