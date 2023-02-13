using CoreCommandLine;
using CoreCommandLine.Attributes;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("target-project", "tp")]
    public class TargetProjectPath : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (AmIPresent(args))
            {
                var foundPath = FindMyValue(args);

                if (foundPath)
                {
                    context.SetTargetProjectPath(foundPath);
                }

                return true;
            }

            return false;
        }

        public override string Description => "Specifies the target project's path.";
    }
}