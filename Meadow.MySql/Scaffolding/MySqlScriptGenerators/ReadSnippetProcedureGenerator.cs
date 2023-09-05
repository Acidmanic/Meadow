using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class ReadSnippetProcedureGenerator<TModel> : ReadSnippetProcedureGenerator
    {
        public ReadSnippetProcedureGenerator(MeadowConfiguration configuration, bool byId) : base(typeof(TModel),
            configuration, byId)
        {
        }
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadSnippetProcedureGenerator : ReadSequenceSnippetProcedureGenerator
    {
        public ReadSnippetProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration, !byId, 0, true, true)
        {
        }
    }
}