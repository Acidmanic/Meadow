using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class ByTemplateSqlGeneratorBase : SqlGeneratorBase
    {
        protected ByTemplateSqlGeneratorBase(IDbTypeNameMapper typeNameMapper,MeadowConfiguration configuration) : 
            base(typeNameMapper,configuration)
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


        protected static Type FilterResultType(Type entityType)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf(entityType);

            if (idLeaf == null)
            {
                throw new Exception($"WARNING: the entity of type {entityType.FullName}" +
                                  $", does not have an identifier field. there for it's not possible " +
                                  $"to have a filter result table created for it.");
            }

            var genericType = typeof(FilterResult<>);

            var filterResultType = genericType.MakeGenericType(idLeaf.Type);
            
            return filterResultType;
        }
        
        
    }
}