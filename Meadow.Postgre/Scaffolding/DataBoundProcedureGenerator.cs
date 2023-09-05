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
        // public DataBoundProcedureGenerator(Type entityType, MeadowConfiguration configuration) : base(entityType,
        //     configuration)
        // {
        // }

        public DataBoundProcedureGenerator(
            SnippetConstruction construction, SnippetConfigurations configuration,
            SnippetConfigurations configurations) : base(construction, configuration, configurations)
        {
        }
        
        protected override bool DelimitByLineNotSplit => false;

        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
        {
            throw new NotImplementedException();
        }

        // protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type type,
        //     MeadowConfiguration configuration)
        // {
        //     return new EntityDataBoundProcedureSnippetGenerator(type, configuration);
        // }
        
    }
}