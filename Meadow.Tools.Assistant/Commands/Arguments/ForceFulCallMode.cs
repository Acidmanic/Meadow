using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Meadow.Scaffolding.Macros;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("forceful","f")]
    public class ForceFulCallMode:ParameterCommandBase
    {

        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.SetCallMode(ExternalToolCallMode.ForceFul);
        }

        public override string Description => "This will set the call mode for apply-macros command, to force full. " +
                                              "A force full call to apply-macros, will cause the tool to replace any " +
                                              "macros with any marking. Macros which are marked as 'By-Force', can only " +
                                              "be replace with this argument.";
    }

}