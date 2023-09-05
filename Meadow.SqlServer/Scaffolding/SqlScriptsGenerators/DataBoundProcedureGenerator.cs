using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.DataBound)]
    public class DataBoundProcedureGenerator : DataBoundProcedureGeneratorBase
    {
        public DataBoundProcedureGenerator(Type entityType, MeadowConfiguration configuration) : base(entityType, configuration)
        {
        }

        protected override bool DelimitByLineNotSplit => false;

        protected override ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type type, MeadowConfiguration configuration)
        {
            return new EntityDataBoundProcedureSnippetGenerator(type, configuration);
        }
    }
}