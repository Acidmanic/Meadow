using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public abstract class BuiltinMacroBase : MacroBase
{
    public override string GenerateCode(params string[] arguments)
    {
        var type = GrabTypeArgument(arguments, 0);

        var catalog = AvailableCodeGeneratorsCatalog(type);

        var assemblingBehaviorBuilder = new AssemblingBehaviorBuilder();

        BuildUpAssemblingBehavior(assemblingBehaviorBuilder);

        var assemblingBehavior = assemblingBehaviorBuilder.Build();

        var matchingCodeGenerators = InstantiateMatchingCodeGenerators(type, assemblingBehavior, catalog);

        var code = AssembleGeneratorsCodes(matchingCodeGenerators);

        return code;
    }

    private List<ICodeGenerator> InstantiateMatchingCodeGenerators(
        Type entityType,
        AssemblingBehavior assemblingBehavior,
        Dictionary<CommonSnippets, Type> catalog)
    {
        var generators = new List<ICodeGenerator>();

        foreach (var order in assemblingBehavior)
        {
            var snippet = order.Snippet;

            if (catalog.ContainsKey(snippet))
            {
                var snippetConfigurations = order.Configurations;

                var snippetType = catalog[snippet];

                var snippetConstruction = new SnippetConstruction
                {
                    EntityType = entityType,
                    MeadowConfiguration = Configuration
                };

                var foundConstructor = snippetType.GetConstructor(new Type[]
                    { typeof(SnippetConstruction), typeof(SnippetConfigurations) });

                var constructionParameters = new object[] { snippetConstruction, snippetConfigurations };

                AddInstancesRegardingIdAwarenessBehavior(generators,
                    foundConstructor!, constructionParameters,
                    snippetConfigurations.IdAwarenessBehavior);
            }
            else
            {
                //TODO: warn unable to find any implementation for snippet
            }
        }

        return generators;
    }

    private void AddInstancesRegardingIdAwarenessBehavior(
        List<ICodeGenerator> generators,
        ConstructorInfo constructor,
        object[] parameters, IdAwarenessBehavior behavior)
    {
        if ((behavior & IdAwarenessBehavior.UseIdAware) != IdAwarenessBehavior.UseNone)
        {
            if (constructor.Invoke(parameters) is IIdAware byId)
            {
                byId.ActById = true;

                generators.Add((ICodeGenerator)byId);
            }

            if (constructor.Invoke(parameters) is IIdAware all)
            {
                all.ActById = false;

                generators.Add((ICodeGenerator)all);
            }
        }
        else
        {
            if (constructor.Invoke(parameters) is ICodeGenerator generator)
            {
                generators.Add(generator);
            }
        }
    }


    protected abstract void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder);


    protected virtual string AssembleGeneratorsCodes(IEnumerable<ICodeGenerator> generators)
    {
        var sb = new StringBuilder();

        foreach (var codeGenerator in generators)
        {
            Title(sb, "<" + Name + " Macro >");

            Append(sb, codeGenerator);

            Title(sb, "</" + Name + " Macro>");
        }

        return sb.ToString();
    }


    private StringBuilder Append(StringBuilder sb, ICodeGenerator cg)
    {
        var code = cg.Generate();

        Title(sb, code.Name);

        sb.AppendLine(code.Text);

        return sb;
    }

    protected void Title(StringBuilder sb, string title)
    {
        sb.AppendLine(LineMacro.CommentLine).Append("\n-- ")
            .AppendLine(title).Append("\n")
            .AppendLine(LineMacro.CommentLine);
    }

    private class NullCodeGenerator : ICodeGenerator
    {
        public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;

        public Code Generate()
        {
            return new Code
            {
                Name = "",
                Text = ""
            };
        }

        public MeadowConfiguration Configuration { get; set; }
    }


    private Result<ConstructorInfo> FindConstructor(Type cgType, bool byIdAware)
    {
        var argumentTypes = byIdAware
            ? new[] { typeof(Type), typeof(MeadowConfiguration), typeof(bool) }
            : new[] { typeof(Type), typeof(MeadowConfiguration) };

        var constructor = cgType.GetConstructor(argumentTypes);

        if (constructor == null)
        {
            var message = byIdAware
                ? "ById-Aware code generators need to have " +
                  "a constructor(Type,MeadowConfiguration,bool) to be adoptable with built-in macros."
                : "ById-Agnostic code generators need to have " +
                  "a constructor(Type,MeadowConfiguration) to be adoptable with built-in macros.";

            return new Result<ConstructorInfo>().FailAndDefaultValue();
        }

        return new Result<ConstructorInfo>(true, constructor);
    }

    private Func<Type, ICodeGenerator> CreateFactory(ConstructorInfo constructor,
        bool byIdAware, bool byId)
    {
        if (byIdAware)
        {
            return tModel => constructor.Invoke(new object[] { tModel, Configuration, byId }) as ICodeGenerator;
        }

        return tModel => constructor.Invoke(new object[] { tModel, Configuration }) as ICodeGenerator;
    }

    protected Dictionary<CommonSnippets, Type> AvailableCodeGeneratorsCatalog(Type entityType)
    {
        var availableBySnippets = new Dictionary<CommonSnippets, Type>();

        foreach (var assembly in LoadedAssemblies)
        {
            // Is ICodeGenerator
            var availableCodeGenerators = assembly.GetAvailableTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(TypeCheck.Implements<ICodeGenerator>);

            foreach (var type in availableCodeGenerators)
            {
                var snippetInfo = type.GetCustomAttribute<CommonSnippetAttribute>();
                // Is Marked as snippet
                if (snippetInfo != null)
                {
                    // Has A Snippet's Constructor
                    var foundConstructor = type.GetConstructor(new Type[]
                        { typeof(SnippetConstruction), typeof(SnippetConfigurations) });

                    if (foundConstructor != null)
                    {
                        // We have a snippet. now validation!
                        // If snippet type should be id-aware, is the implementation id-aware too?
                        var expectedToBeIdAware = new CommonSnippetsInfo().IsIdAware(snippetInfo.SnippetType);

                        var actuallyIsIdAware = TypeCheck.Implements<IIdAware>(type);

                        if (!expectedToBeIdAware || actuallyIsIdAware)
                        {
                            availableBySnippets.Add(snippetInfo.SnippetType, type);
                        }
                    }
                }
            }
        }

        return availableBySnippets;
    }
}