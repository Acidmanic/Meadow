using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class TableScriptSnippetGenerator<TEntity> : TableScriptSnippetGenerator
    {
        public TableScriptSnippetGenerator(MeadowConfiguration configuration) : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.CreateTable)]
    public class TableScriptSnippetGenerator : TableScriptSnippetGeneratorBase
    {
        public TableScriptSnippetGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }
    }

    public abstract class TableScriptSnippetGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        private readonly Type _type;

        public TableScriptSnippetGeneratorBase(Type type, MeadowConfiguration configuration) : base(new MySqlDbTypeNameMapper(),
            configuration)
        {
            _type = type;
        }


        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyDropping = GenerateKey();
        private readonly string _keyCreation = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process(_type);

            var dropping = "";
            var creation = "CREATE TABLE";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                dropping = "DROP TABLE IF EXISTS " + process.NameConvention.TableName + ";\n";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creation = "CREATE TABLE IF NOT EXISTS";
            }

            replacementList.Add(_keyDropping, dropping);
            replacementList.Add(_keyCreation, creation);

            // replacementList.Add(_keyTableName,
            //                IsDatabaseObjectNameForced ? ForcedDatabaseObjectName : process.NameConvention.TableName);
            
            replacementList.Add(_keyTableName,GetTableName(process));

            var parameters = GetParameters(process);

            replacementList.Add(_keyParameters, parameters);
        }


        protected virtual string GetTableName(ProcessedType process)
        {
            return process.NameConvention.TableName;
        }
        

        private string GetParameters(ProcessedType process)
        {
            var parameters = string.Join(',', process.NoneIdParameters.Select(p => p.Name + " " + p.Type));

            if (process.HasId)
            {
                var idParam = process.IdParameter.Name + " " + process.IdParameter.Type;

                idParam += process.IdField.IsUnique ? " NOT NULL PRIMARY KEY" : "";

                idParam += process.IdField.IsAutoValued ? " AUTO_INCREMENT" : "";

                idParam += process.NoneIdParameters.Count > 0 ? "," : "";

                parameters = idParam + parameters;
            }

            return parameters;
        }

        protected override string Template => $@"
{_keyDropping}{_keyCreation} {_keyTableName}
(
    {_keyParameters}
);
".Trim();
    }
}