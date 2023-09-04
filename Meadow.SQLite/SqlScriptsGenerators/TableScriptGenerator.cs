using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptGenerator : TableScriptGeneratorBase
    {
        public TableScriptGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }
    }
    
    
    
    public abstract class TableScriptGeneratorBase : ByTemplateSqlGeneratorBase
    {

        protected ProcessedType ProcessedType { get; }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdParameters = GenerateKey();
        private readonly string _keyNoneIdParameters = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();

        public TableScriptGeneratorBase(Type type, MeadowConfiguration configuration) : base(new SqLiteTypeNameMapper(),
            configuration)
        {
            ProcessedType = Process(type);
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

        protected virtual string GetTableName()
        {
            return ProcessedType.NameConvention.TableName;
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
{LineMacro.CommentLine}
-- SPLIT
{LineMacro.CommentLine}
".Trim();
    }
}