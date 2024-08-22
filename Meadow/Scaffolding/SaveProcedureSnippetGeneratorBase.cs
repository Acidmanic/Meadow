using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding
{
    public abstract class SaveProcedureSnippetGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {

        public SaveProcedureSnippetGeneratorBase(SnippetConstruction construction, SnippetConfigurations configurations, SnippetExecution execution) : base(construction, configurations, execution)
        {
        }
        
        private readonly string _preKeyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();

        private readonly string _preKeyInsertColumns = GenerateKey();
        private readonly string _preKeyWhereClause = GenerateKey();
        private readonly string _preKeyInsertValues = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        private readonly string _preKeyUpdates = GenerateKey();
        

        private string CreatePreReplacedTemplate(Dictionary<string, string> replacements)
        {
            var p = RawTemplate;

            foreach (var replacement in replacements)
            {
                p = p.Replace(replacement.Key, replacement.Value);
            }
            
            return p + LineTemplate;
        }

        
        private const string LineTemplate =
            "\n-- ---------------------------------------------------------------------------------------------------------------------\n";



        protected override string Template =>
            string.Join(LineTemplate, CreatePerProcedureReplacements().Select(CreatePreReplacedTemplate));

        protected abstract List<Dictionary<string, string>> CreatePerProcedureReplacements();
        
        protected abstract string RawTemplate { get; }
        
       
    }
}