using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Meadow.Scaffolding.Macros;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("weak","w")]
    public class WeakCallMode:ParameterCommandBase
    {

        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.SetCallMode(ExternalToolCallMode.Weak);
        }

        public override string Description => "This will set the call mode for apply-macros command, to weak mode. " +
                                              "A weak call to apply-macros, will cause the tool to only replace " +
                                              "macros with 'Always' marking. Macros which are marked as By-Force, or " +
                                              "macros that are not marked (regulars) will not be touched.";
    }

}