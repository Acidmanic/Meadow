using System;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Snippets.Extensions;
/// <summary>
/// This class aims to ease rendering snippets for client codes which need to generate script out of snippets
/// outside of macros, like DataAccess cores trying to construct meadow databases and etc.
/// </summary>
public static class SnippetCodeGeneratingExtensions
{

    public static string Generate(this ISnippet snippet,
        MeadowConfiguration configuration,
        Action<SnippetToolboxBuilder>? configure = null)
        => Generate(snippet, configuration, typeof(object), configure);

    public static string Generate(this ISnippet snippet,
        MeadowConfiguration configuration,
        Type entityType,
        Action<SnippetToolboxBuilder>? configure = null)
    {
        var builder = new SnippetToolboxBuilder(configuration, entityType);

        if (configure is { }) configure(builder);

        snippet.Toolbox = builder.Build();
        
        var translator = new SnippetTranslator();

        var script = translator.Translate(snippet);

        return script;
    }
}