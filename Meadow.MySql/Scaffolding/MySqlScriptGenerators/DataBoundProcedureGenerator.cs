using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.DataBound)]
    public class DataBoundProcedureGenerator : DataBoundProcedureGeneratorBase
    {
        
        protected override bool DelimitByLineNotSplit => true;

        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
        {
            return new EntityDataBoundProcedureSnippetGenerator(construction,configurations);
        }


        public DataBoundProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configuration, SnippetConfigurations configurations) : base(construction, configuration, configurations)
        {
            
        }
    }
}