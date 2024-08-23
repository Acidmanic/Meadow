using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.Scaffolding.Macros.SnippetComposed;

public abstract class SnippetComposedMacroBase:MacroBase
{
    public override string GenerateCode(params string[] arguments)
    {
        var assemblingBehaviorBuilder = new AssemblingBehaviorBuilder();
        
        BuildUpAssemblingBehavior(assemblingBehaviorBuilder);
        
        var assemblingBehavior = assemblingBehaviorBuilder.Build();

        var script = new StringBuilder();

        var menu = ConstructSnippetsMenu();
        
        assemblingBehavior.ForEach(b =>  script.AppendLine(PlaceSnippetCode(b,menu)));

        return script.ToString();
    }

    private string PlaceSnippetCode(SnippetOrder snippetOrder, Dictionary<CommonSnippets,Type> menu)
    {
        if (menu.ContainsKey(snippetOrder.Snippet))
        {
            var snippetInstance = InstantiateSnippetOrDefault(menu[snippetOrder.Snippet]);

            if (snippetInstance is { } snippet)
            {
                var translator = new SnippetTranslator();

                var translated = translator.Translate(snippet);

                return translated;
            }
        }

        return string.Empty;
    }

    private ISnippet? InstantiateSnippetOrDefault(Type type)
    {
        var constructor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);

        if (constructor is { } c)
        {
            try
            {
                var instance = c.Invoke(Array.Empty<object>());

                return instance as ISnippet;
            }
            catch 
            {
                /*Ignore*/
            }
        }

        return null;
    }


    private List<Type> FindAllSnippets()
    {
        var types = new List<Type>();
        
        var dataAccessAssembly = MeadowEngine.DataAccessAssembly();

        if (dataAccessAssembly is { } assembly)
        {
            return assembly.GetAvailableTypes()
                .Where(TypeCheck.Implements<ISnippet>).ToList();
        }

        return types;
    }

    private Dictionary<CommonSnippets, Type> ConstructSnippetsMenu()
    {

        var menu = new Dictionary<CommonSnippets, Type>();
        
        var snippetTypes = FindAllSnippets();

        foreach (var snippetType in snippetTypes)
        {
            var commonAttribute = snippetType.GetCustomAttribute<CommonSnippetAttribute>();

            if (commonAttribute is { } attribute)
            {

                if (menu.ContainsKey(attribute.SnippetType)) menu.Remove(attribute.SnippetType);
                
                menu.Add(attribute.SnippetType,snippetType);
            }
        }

        return menu;
    }
    
    protected abstract void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder);
}