using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.DataBound)]
    public class DataBoundProcedureGenerator : DataBoundProcedureGeneratorBase
    {
        public DataBoundProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }
       
        protected override bool DelimitByLineNotSplit => false;

        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(
            SnippetConstruction construction,
            SnippetConfigurations configurations)
        {
            return new EntityDataBoundProcedureSnippetGenerator()
        }

        // protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type type,
        //     MeadowConfiguration configuration)
        // {
        //     return new EntityDataBoundProcedureSnippetGenerator(type, configuration);
        // }

        
    }
}