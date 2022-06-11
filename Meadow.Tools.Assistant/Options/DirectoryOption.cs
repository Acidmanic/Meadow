using ConsoleAppFramework;

namespace Meadow.Tools.Assistant.Options
{
    public class DirectoryOption:OptionAttribute
    {
        public DirectoryOption() : base("d", "The path to target Meadow Project")
        {
        }

    }
}