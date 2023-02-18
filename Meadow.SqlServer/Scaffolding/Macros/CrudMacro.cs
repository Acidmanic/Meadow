using System;
using System.Text;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class CrudMacro : MacroBase
    {
        public override string Name { get; } = "Crud";

        private readonly string _line = new LineMacro().GenerateCode();

        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);


            var sb = new StringBuilder();

            Append(sb,"Entity Table", new TableScriptGenerator(type));

            sb.AppendLine(_line).Append("-- ")
                .AppendLine("SPLIT")
                .AppendLine(_line);
            
            Append(sb,"Insert New Entity", new InsertProcedureGenerator(type));

            Append(sb, "Read All Entities", new ReadProcedureGenerator(type, false));

            Append(sb,"Read Entity By Id", new ReadProcedureGenerator(type, true));
            
            Append(sb,"Delete All Entities", new DeleteProcedureGenerator(type, true));
            
            Append(sb,"Delete Entity ById", new DeleteProcedureGenerator(type, false));
            
            Append(sb,"Update Existing Entity", new UpdateProcedureGenerator(type));
            
            Append(sb,"Save Procedure (Update Existing Otherwise Insert)", new SaveProcedureGenerator(type));

            sb.AppendLine(_line).AppendLine(_line).AppendLine(_line);
                
            return sb.ToString();
        }


        private void Append(StringBuilder sb, string title, ICodeGenerator cd)
        {
            sb.AppendLine(_line).Append("-- ").AppendLine(title)
                .AppendLine(_line);

            var code = cd.Generate().Text;

            sb.AppendLine(code);
        }
    }
}