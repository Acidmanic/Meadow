using ConsoleAppFramework;

namespace Meadow.Tools.Assistant.Options
{
    public class DelimiterOptions : OptionAttribute
    {
        public DelimiterOptions() : base("-dl", "A character for separating values.")
        {
        }
    }
}