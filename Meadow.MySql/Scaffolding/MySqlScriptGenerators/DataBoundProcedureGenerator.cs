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
        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type entityType, MeadowConfiguration configuration)
        {
            return new EntityDataBoundProcedureSnippetGenerator(entityType, configuration);
        }

        public DataBoundProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) 
            : base(construction, configurations)
        {
        }
    }
}