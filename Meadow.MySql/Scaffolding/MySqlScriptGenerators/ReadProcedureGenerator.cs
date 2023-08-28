using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class ReadProcedureGenerator<TModel> : ReadProcedureGenerator
    {
        public ReadProcedureGenerator(MeadowConfiguration configuration, bool byId) : base(typeof(TModel),
            configuration, byId)
        {
        }
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : ReadSequenceProcedureGenerator
    {
        public ReadProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration, !byId, 0, true, true)
        {
        }
    }
}