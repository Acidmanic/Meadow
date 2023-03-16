using CoreCommandLine;
using CoreCommandLine.Attributes;
using Meadow.Tools.Assistant.Commands;
using Meadow.Tools.Assistant.Commands.ApplyMacros;
using Meadow.Tools.Assistant.Commands.ExtractBuildScripts;

namespace Meadow.Tools.Assistant
{
    [Subcommands(
        typeof(ApplyMacros),
        typeof(ExtractBuildupScripts)
        )]
    public class MatApplication:CommandLineApplication
    {
        
    }
}