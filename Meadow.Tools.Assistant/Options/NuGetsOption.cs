using ConsoleAppFramework;

namespace Meadow.Tools.Assistant.Options
{
    public class NuGetsOption : OptionAttribute
    {
        public NuGetsOption() : base("n", "comma separated List of Local nuget directories")
        {
        }
    }
}