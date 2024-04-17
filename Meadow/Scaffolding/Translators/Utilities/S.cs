namespace Meadow.Scaffolding.Translators.Utilities;

internal static class S
{
    public static string Indent(int count)
    {
        var indent = "";

        for (int i = 0; i < count; i++)
        {
            indent += "    ";
        }

        return indent;
    }
}