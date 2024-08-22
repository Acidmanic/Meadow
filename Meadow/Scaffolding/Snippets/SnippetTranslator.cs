using System;
using System.Collections;
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
                var objectCollection = ev.Read(child.GetFullName());

                var sb = new StringBuilder();
                
                if (objectCollection is IEnumerable enumerable)
                {
                    foreach (var collectable in enumerable)
                    {
                        if (collectable is { } collectableValue)
                        {
                            if (collectableValue is ISnippet subSnippet)
                            {
                                sb.AppendLine(Translate(subSnippet));
                            }
                            else
                            {
                                sb.AppendLine($"{collectableValue}");
                            }
                        }
                    }
                    if (sb.Length > 0)
                    {
                        replacements.Add("{" + child.Name + "}", $"{sb}");
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