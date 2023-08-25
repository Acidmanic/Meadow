using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly string _line =
            "-- ----------------------------------------------------------" +
            "------------------------------------------------------------";

        private ProcessedType ProcessedType { get; }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdParameters = GenerateKey();
        private readonly string _keyNoneIdParameters = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();

        public TableScriptGenerator(Type type) : base(new SqLiteTypeNameMapper())
        {
            ProcessedType = Process(type);
        }


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var creationHeader = GetCreationHeader();

            replacementList.Add(_keyCreationHeader, creationHeader);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var idParameters = "\n";

            if (ProcessedType.HasId)
            {
                idParameters += "    " + ParameterNameTypeJoint(ProcessedType.IdParameter);

                idParameters += " NOT NULL PRIMARY KEY";

                if (ProcessedType.IdField.IsAutoValued && IsNumeric(ProcessedType.IdField.Type))
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

        private string GetCreationHeader()
        {
            var creationHeader = "CREATE TABLE";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "DROP TABLE IF EXISTS " + ProcessedType.NameConvention.TableName + ";" +
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
{_line}
-- SPLIT
{_line}
".Trim();
    }
}