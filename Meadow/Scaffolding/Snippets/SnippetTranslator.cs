using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Snippets;

public class SnippetTranslator
{
    public string Translate(ISnippet snippet)
    {
        var ev = new ObjectEvaluator(snippet);

        var children = ev.RootNode.GetChildren();

        var replacements = new Dictionary<string, string>();

        bool IsSnippetLeaf(AccessNode n) => n.IsLeaf || TypeCheck.Implements<ISnippet>(n.Type);

        foreach (var child in children)
        {
            if (IsSnippetLeaf(child))
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

                var sb = new StringBuilder();
                
                foreach (var collectable in collectables)
                {
                    if (IsSnippetLeaf(collectable))
                    {
                        var collectableValue = ev.Read(collectable.GetFullName());

                        if (collectableValue as ISnippet is { } collectableSubSnippet)
                        {
                            sb.AppendLine(Translate(collectableSubSnippet));
                        }
                        else if (collectableValue is { } cv)
                        {
                            sb.AppendLine($"{cv}");
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    replacements.Add("{" + child.Name + "}", $"{sb}");
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