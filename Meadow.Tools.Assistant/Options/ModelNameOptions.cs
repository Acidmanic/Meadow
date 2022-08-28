using ConsoleAppFramework;

namespace Meadow.Tools.Assistant.Options
{
    public class ModelNameOptions : OptionAttribute
    {
        public ModelNameOptions() : base("m", "Class name or FullName of the model")
        {
        }
    }
}