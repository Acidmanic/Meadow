using System.Text;
using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
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
            
            Append(sb,"Insert New Entity", new InsertProcedureGenerator(type));
            
            Append(sb,"Read All Entities", new ReadSequenceProcedureGenerator
                (type, true, 0, true));

            Append(sb,"Read Entity By Id", new ReadSequenceProcedureGenerator
                (type, false, 0, true));
            
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