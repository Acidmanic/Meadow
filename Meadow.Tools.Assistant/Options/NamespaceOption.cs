using ConsoleAppFramework;

namespace Meadow.Tools.Assistant.Options
{
    public class NamespaceOption : OptionAttribute
    {
        public NamespaceOption() : base("ns",
            "Root namespace to search for models. The default value would be RootNamespace of the project.")
        {
        }
    }
}