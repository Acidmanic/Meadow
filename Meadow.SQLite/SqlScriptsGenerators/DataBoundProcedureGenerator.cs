using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.DataBound)]
    public class DataBoundProcedureGenerator : DataBoundProcedureGeneratorBase
    {
        public DataBoundProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }

        protected override bool DelimitByLineNotSplit => false;

        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type type, MeadowConfiguration configuration)
        {
            return new EntityDataBoundProcedureSnippetGenerator(type, configuration);
        }
    }
}