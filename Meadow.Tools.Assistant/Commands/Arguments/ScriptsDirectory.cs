using CoreCommandLine;
using CoreCommandLine.Attributes;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("scripts-directory", "sd")]
    public class ScriptsDirectory : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (AmIPresent(args))
            {
                var foundPath = FindMyValue(args);

                if (foundPath)
                {
                    context.SetScriptsDirectoryPath(foundPath);
                }

                return true;
            }

            return false;
        }

        public override string Description => "Specifies the directory containing build-up scripts sql files.";
    }
}