using CoreCommandLine;
using CoreCommandLine.Attributes;
using Meadow.Tools.Assistant.Commands.Macros;

namespace Meadow.Tools.Assistant
{
    [Subcommands(
        typeof(ApplyMacros)
        )]
    public class MatApplication:CommandLineApplication
    {
        
    }
}