using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Scaffolding.Snippets;

public class SnippetTranslator
{
    public string Translate(ISnippet snippet)
    {
        var ev = new ObjectEvaluator(snippet);

        var children = ev.RootNode.GetChildren();

        var replacements = new Dictionary<string, string>();

        Func<AccessNode, bool> isSnippetLeaf = n => n.IsLeaf || TypeCheck.Implements<ISnippet>(n.Type);

        foreach (var child in children)
        {
            if (isSnippetLeaf(child))
            {
                var value = ev.Read(child.GetFullName());

                if (value as ISnippet is { } subSnippet)
                {
                    replacements.Add("{" + child.Name + "}", $"{Translate(subSnippet)}");
                }
                else if (value is { } v)
                {
                    replacements.Add("{" + child.Name + "}", $"{v}");
                }
            }
            else if (child.IsCollection)
            {
                var collectables = child.GetChildren();

                foreach (var collectable in collectables)
                {
                    if (isSnippetLeaf(collectable))
                    {
                        var collectableValue = ev.Read(collectable.GetFullName());

                        if (collectableValue as ISnippet is { } collectableSubSnippet)
                        {
                            replacements.Add("{" + collectable.Name + "}", $"{Translate(collectableSubSnippet)}");
                        }
                        else if (collectableValue is { } cv)
                        {
                            replacements.Add("{" + collectable.Name + "}", $"{cv}");
                        }
                    }
                }
            }
        }

        var translated = snippet.Template;

        foreach (var replacement in replacements)
        {
            translated = translated.Replace(replacement.Key, replacement.Value);
        }

        return translated;
    }
}