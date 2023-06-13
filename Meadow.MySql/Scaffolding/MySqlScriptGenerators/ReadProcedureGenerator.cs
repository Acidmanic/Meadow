using System;
using Meadow.Scaffolding.Attributes;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{

    public class ReadProcedureGenerator<TModel> : ReadProcedureGenerator
    {
        public ReadProcedureGenerator(bool byId) : base(typeof(TModel), byId)
        {
        }
    }
    
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : ReadSequenceProcedureGenerator
    {
        public ReadProcedureGenerator(Type type, bool byId) : base(type, !byId, 0, true,true)
        {
        }
    }
}