using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class ByTemplateSqlGeneratorBase : SqlGeneratorBase
    {
        
        public MeadowConfiguration Configuration { get; set; }
        
        
        protected ByTemplateSqlGeneratorBase(IDbTypeNameMapper typeNameMapper) : base(typeNameMapper)
        {
        }

        protected abstract void AddReplacements(Dictionary<string, string> replacementList);

        protected abstract string Template { get; }

        public override Code Generate()
        {
            var replacements = new Dictionary<string, string>();

            AddReplacements(replacements);

            var code = Replace(Template, replacements);

            return new Code
            {
                Name = this.GetType().Name,
                Text = code
            };
        }

        private string Replace(string template, Dictionary<string, string> replacements)
        {
            foreach (var item in replacements)
            {
                template = template.Replace(item.Key, item.Value);
            }

            return template;
        }

        protected static string GenerateKey()
        {
            return "{" + Guid.NewGuid().ToString() + "}";
        }

        protected void LogUnSupportedRepetitionHandling(string databaseName,
            string dbObjectTypeName,
            RepetitionHandling handling)
        {
            Console.WriteLine($"WARNING: {databaseName} Does not support skip repetition handling strategy for " +
                              $"{dbObjectTypeName}, so this code generator snippet would handle repetition as" +
                              $" 'create' behavior.");
        }

        protected void LogUnSupportedRepetitionHandling(string snippetName)
        {
            Console.WriteLine($"WARNING: {snippetName} Snippets does not support repetition handling.");
        }
    }
}