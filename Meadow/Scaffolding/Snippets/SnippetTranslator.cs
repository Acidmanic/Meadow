using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        var methods = snippet.GetType().GetMethods().Where(IsReplaceMethod).ToList();

        foreach (var method in methods)
        {
            while (true)
            {
                var foundReplacement = ParseReplacement(method, translated);

                if (foundReplacement)
                {
                    var invoked = Invoke(snippet, method, foundReplacement.Value);

                    if (invoked)
                    {
                        translated = Replace(foundReplacement.Value, invoked.Value, translated);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return translated;
    }

    private string Replace(MethodReplacement replacement, string replacementValue, string content)
    {
        var pre = content.Substring(0, replacement.Index);

        var endIndex = replacement.Index + replacement.Length;

        var post = content.Substring(endIndex, content.Length - endIndex);

        return pre + replacementValue + post;
    }

    private Result<string> Invoke(object owner, MethodInfo method, MethodReplacement replacement)
    {
        try
        {
            object[] methodParameters = { };

            if (method.GetParameters().Length == 1)
            {
                methodParameters = new object[] { replacement.Parameter };
            }

            var invokeResult = method.Invoke(owner, methodParameters);

            if (invokeResult is { } invokeResultObject)
            {
                if (invokeResultObject is string invokeResultString)
                {
                    return new Result<string>(true, invokeResultString);
                }

                if (invokeResultObject is ISnippet snippet)
                {
                    var translated = Translate(snippet);

                    return new Result<string>(true, translated);
                }
            }
        }
        catch (Exception e)
        {
            /* ignore */
        }

        return new Result<string>();
    }

    private bool IsReplaceMethod(MethodInfo m)
    {
        if (m.ReturnType == typeof(string) || TypeCheck.Implements<ISnippet>(m.ReturnType))
        {
            var parameters = m.GetParameters();

            if (parameters.Length == 0) return true;

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string)) return true;
        }

        return false;
    }


    private record MethodReplacement(int Index, int Length, string Parameter, bool HasParameter);


    private Result<MethodReplacement> ParseReplacement(MethodInfo method, string content)
    {
        string startTag = "{" + method.Name + "}";

        string endTag = "{/" + method.Name + "}";

        int searchStart = 0;
        
        while (searchStart < content.Length)
        {
            var startIndex = content.IndexOf(startTag, searchStart, StringComparison.Ordinal);

            if (startIndex > -1)
            {
                var parameterStartIndex = startIndex + startTag.Length;

                var endStartIndex = content.IndexOf(endTag, parameterStartIndex, StringComparison.Ordinal);

                if (endStartIndex > startIndex)
                {
                    var parameterLength = endStartIndex - parameterStartIndex;

                    var parameter = content.Substring(parameterStartIndex, parameterLength);

                    var replacementEndIndex = endStartIndex + endTag.Length;

                    var replacementLength = replacementEndIndex - startIndex;

                    var hasParams = parameter.Length > 0;

                    if (hasParams == (method.GetParameters().Length > 0))
                    {
                        return new Result<MethodReplacement>(true, new MethodReplacement(startIndex, replacementLength, parameter, hasParams));
                    }

                    searchStart = replacementEndIndex;
                }
                else
                {
                    searchStart = startIndex + 1;
                }
            }
            else
            {
                searchStart += content.Length;
            }
        }

        return new Result<MethodReplacement>().FailAndDefaultValue();
    }
}