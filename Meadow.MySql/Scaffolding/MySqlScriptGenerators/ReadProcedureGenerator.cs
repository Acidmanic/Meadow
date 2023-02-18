using System;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : ReadSequenceProcedureGenerator
    {
        public ReadProcedureGenerator(Type type, bool byId) : base(type, !byId, 0, true)
        {
        }
    }
}