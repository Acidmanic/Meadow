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
        var entityType = GrabTypeArgument(arguments, 0);
        
        var assemblingBehaviorBuilder = new AssemblingBehaviorBuilder();
        
        BuildUpAssemblingBehavior(assemblingBehaviorBuilder);
        
        var assemblingBehavior = assemblingBehaviorBuilder.Build();

        var script = new StringBuilder();

        var menu = ConstructSnippetsMenu();
        
        assemblingBehavior.ForEach(b =>  script.AppendLine(PlaceSnippetCode(b,menu,entityType)));

        return script.ToString();
    }

    private string PlaceSnippetCode(SnippetOrder snippetOrder, Dictionary<CommonSnippets,Type> menu,Type entityType)
    {
        if (menu.ContainsKey(snippetOrder.Snippet))
        {
            var snippetInstance = InstantiateSnippetOrDefault(menu[snippetOrder.Snippet]);

            if (snippetInstance is { } snippet)
            {
                var translator = new SnippetTranslator();
                
                snippet.Toolbox = CreateToolBox(entityType,snippetOrder);
                
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

    private SnippetToolbox CreateToolBox(Type entityType, SnippetOrder snippetOrder)
    {
        var snippetConstruction = new SnippetConstruction
        {
            EntityType = entityType,
            MeadowConfiguration = Configuration
        };

        return new SnippetToolbox(snippetConstruction, snippetOrder.Configurations);
    }
    
    protected abstract void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder);
}