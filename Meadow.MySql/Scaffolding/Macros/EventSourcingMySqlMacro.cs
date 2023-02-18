using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
{

    public class EventSourcingMySqlMacro : MacroBase
    {
        public override string Name { get; } = "EventStreamOut";
        
        public override string GenerateCode(params string[] arguments)
        {

            var entityType = GrabTypeArgument(arguments, 0);

            var generator = new EventStreamSqlScriptGenerator(entityType);

            var code = generator.Generate().Text;
            
            return code;
        }

        
    }
}