using System;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public abstract class ProcedureGenerator : SqlGeneratorBase
    {
        protected ProcedureGenerator(Type type) : base(type)
        {
        }

        public string ProcedureName => GetProcedureName();

        public override string SqlObjectName => ProcedureName;


        protected abstract string GetProcedureName();


        public override Code Generate(bool alreadyExists)
        {
            var script = GenerateScript(alreadyExists, CreateKeyWord(alreadyExists));

            return new Code
                {
                    Name = ProcedureName,
                    Text = script
                };
        }

        protected abstract string GenerateScript(bool alreadyExists, string createKeyword);
    }
}