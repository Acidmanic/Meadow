using CoreCommandLine;
using CoreCommandLine.Attributes;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    
    [CommandName("meadow-configuration-provider", "mcp")]
    public class MeadowConfigurationProvider : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (AmIPresent(args))
            {
                var foundValue = FindMyValue(args);

                if (foundValue)
                {
                    context.SetMeadowConfigurationProvideTypeName(foundValue);
                }

                return true;
            }

            return false;
        }

        public override string Description => "Specifies the exact implementation type for IMeadowConfigurationProvider by it's Fullname";
    }
}