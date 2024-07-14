using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class ByTemplateSqlSnippetGeneratorBase : SqlSnippetGeneratorBase
    {
        protected interface ISupportDeclaration
        {
            void NotSupported(RepetitionHandling repetitionHandling);

            void NotSupportedRepetitionHandling();

            void NotSupportedEntityTypeOverriding();

            void NotSupportedDbObjectNameOverriding();
        }

        /// <summary>
        /// This class will only be used to create an easy way for driven classes to warn the client code
        /// when a feature is not supported by their implementation. 
        /// </summary>
        private class SupportList : ISupportDeclaration
        {
            public List<RepetitionHandling> UnSupportedRepetitions { get; } = new List<RepetitionHandling>();

            public bool SupportsDbObjectNameOverriding { get; set; } = true;

            public bool SupportsEntityTypeOverriding { get; set; } = true;

            public void NotSupported(RepetitionHandling repetitionHandling)
            {
                UnSupportedRepetitions.Add(repetitionHandling);
            }

            public void NotSupportedRepetitionHandling()
            {
                UnSupportedRepetitions.Add(RepetitionHandling.Alter);
                UnSupportedRepetitions.Add(RepetitionHandling.Skip);
            }

            public void NotSupportedEntityTypeOverriding()
            {
                SupportsEntityTypeOverriding = false;
            }

            public void NotSupportedDbObjectNameOverriding()
            {
                SupportsDbObjectNameOverriding = false;
            }
        }


        protected ByTemplateSqlSnippetGeneratorBase
        (
            SnippetConstruction construction,
            SnippetConfigurations configurations,
            SnippetExecution execution) :
            base(construction, configurations,execution)
        {
            var dec = new SupportList();
            // ReSharper disable once VirtualMemberCallInConstructor
            DeclareUnSupportedFeatures(dec);

            var myName = GetType().Name;

            if (!dec.SupportsEntityTypeOverriding && configurations.OverrideEntityType)
            {
                Console.WriteLine($"WARNING: {myName} Does not support overriding EntityType. ");
            }

            if (!dec.SupportsDbObjectNameOverriding && configurations.OverrideDbObjectName)
            {
                Console.WriteLine($"WARNING: {myName} Does not support overriding database object name ");
            }

            if (dec.UnSupportedRepetitions.Contains(configurations.RepetitionHandling))
            {
                Console.WriteLine($"WARNING: {myName} Does not support {configurations.RepetitionHandling.ToString()}" +
                                  $" repetition handling strategy for type{construction.EntityType.Name}, " +
                                  $"so this code generator snippet would handle repetition as 'create' behavior.");
            }

            if (configurations.IdAwarenessBehavior is IdAwarenessBehavior.UseIdAware
                && !TypeCheck.Implements<IIdAware>(this.GetType()))
            {
                Console.WriteLine($"ERROR: {this.GetType().FullName} Does not support id-awareness since it's not " +
                                  $"implementing IIdAware interface, yet it's instantiated to behave id-aware.");
            }
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


        protected virtual void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
        }
    }
}