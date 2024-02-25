using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd010FullTreeViewManualReference
    {
        private class Parent
        {
            public string Id { get; set; }

            public List<Child> Children { get; set; }
        }

        [OwnerName("Parents")]
        [OneToMany(nameof(ChildProxy.ParentId))]
        private class ParentProxy
        {
            [UniqueMember] public string Id { get; set; }

            public List<ChildProxy> Children { get; set; }
        }


        private class Child
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string ParentId { get; set; }
        }

        [OwnerName("Children")]
        private class ChildProxy
        {
            [UniqueMember] public string Id { get; set; }

            public string ParentId { get; set; }

            public string Name { get; set; }
        }


        private void PrintForType<T>()
        {
            var construct = new SnippetConstruction
            {
                EntityType = typeof(T),
                MeadowConfiguration = new MeadowConfiguration()
                {
                    TableNameProvider = new PluralDataOwnerNameProvider(),
                    DatabaseFieldNameDelimiter = '_'
                }
            };

            var configurations = new SnippetConfigurations
            {
                OverrideEntityType = { Success = false },
                OverrideDbObjectName = { Success = false }
            };

            var fulltreeGen = new FullTreeViewCodeGenerator(construct, configurations);

            var code = fulltreeGen.Generate();

            var codeText = code.Text;

            Console.WriteLine(codeText);
        }


        public void Main()
        {
            PrintForType<ParentProxy>();
            PrintForType<Parent>();
        }
    }
}