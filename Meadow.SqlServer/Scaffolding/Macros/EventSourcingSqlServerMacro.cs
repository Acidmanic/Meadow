using System;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{

    public class EventSourcingSqlServerMacro : MacroBase
    {
        public override string Name { get; } = "EventStream";
        

        public override string GenerateCode(params string[] arguments)
        {

            var entityType = GrabTypeArgument(arguments, 0);

            var code = new EventStreamSqlScriptGenerator(entityType).Generate().Text;

            return code;
        }

    }
}