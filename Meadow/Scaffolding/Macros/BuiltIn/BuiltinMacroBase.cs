using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public abstract class BuiltinMacroBase : MacroBase
{
    private readonly string _line = "-- ---------------------------------------------------------" +
                                    "------------------------------------------------------------";

    protected class CodeGeneratorConstruction
    {
        public Func<Type, ICodeGenerator> ByIdCodeGenerator { get; set; }

        public Func<Type, ICodeGenerator> AllCodeGenerator { get; set; }

        public Func<Type, ICodeGenerator> IdAgnosticCodeGenerator { get; set; }


        public bool IdAware { get; set; }
    }

    public override string GenerateCode(params string[] arguments)
    {
        var type = GrabTypeArgument(arguments, 0);

        var catalog = AvailableCodeGeneratorsCatalog();

        var code = GenerateCode(type, catalog);

        return code;
    }


    protected abstract Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior();


    protected virtual string GenerateCode(Type type, Dictionary<CommonSnippets, CodeGeneratorConstruction> cgCatalog)
    {
        var behaviorsBySnippets = GetAssemblyBehavior();

        var sb = AssembleCodeGenerators(new StringBuilder(), type, cgCatalog, behaviorsBySnippets);

        Title(sb, "</" + Name + ">");

        return sb.ToString();
    }


    private StringBuilder AssembleCodeGenerators(StringBuilder sb, Type entityType,
        Dictionary<CommonSnippets, CodeGeneratorConstruction> generatorsCatalog,
        Dictionary<CommonSnippets, SnippetInstantiationInstruction> codeGenerateBehaviors)
    {
        
        foreach (var behaviorItem in codeGenerateBehaviors)
        {
            var snippet = behaviorItem.Key;

            var finalEntityType = behaviorItem.Value.OverrideEntity ? behaviorItem.Value.OverrideEntity.Value : entityType;

            if (generatorsCatalog.ContainsKey(snippet))
            {
                var behavior = behaviorItem.Value;

                var construction = generatorsCatalog[snippet];

                if (behavior.CodeGenerateBehavior.Is(CodeGenerateBehavior.UseIdAgnostic))
                {
                    if (!construction.IdAware)
                    {
                        Append(sb, construction.IdAgnosticCodeGenerator(finalEntityType));
                    }
                }

                if (behavior.CodeGenerateBehavior.Is(CodeGenerateBehavior.UseById))
                {
                    if (construction.IdAware)
                    {
                        Append(sb, construction.ByIdCodeGenerator(finalEntityType));
                    }
                }

                if (behavior.CodeGenerateBehavior.Is(CodeGenerateBehavior.UseAll))
                {
                    if (construction.IdAware)
                    {
                        Append(sb, construction.AllCodeGenerator(finalEntityType));
                    }
                }
            }
        }

        return sb;
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
        sb.AppendLine(_line).Append("-- ").AppendLine(title)
            .AppendLine(_line);
    }

    private class NullCodeGenerator : ICodeGenerator
    {
        public Code Generate()
        {
            return new Code
            {
                Name = "",
                Text = ""
            };
        }
    }


    private Result<ConstructorInfo> FindConstructor(Type cgType, bool byIdAware)
    {
        var argumentTypes = byIdAware ? new[] { typeof(Type), typeof(bool) } : new[] { typeof(Type) };

        var constructor = cgType.GetConstructor(argumentTypes);

        if (constructor == null)
        {
            var message = byIdAware
                ? "ById-Aware code generators need to have " +
                  "a constructor(Type,bool) to be adoptable with built-in macros."
                : "ById-Agnostic code generators need to have " +
                  "a constructor(Type) to be adoptable with built-in macros.";

            return new Result<ConstructorInfo>().FailAndDefaultValue();
        }

        return new Result<ConstructorInfo>(true, constructor);
    }

    private Func<Type, ICodeGenerator> CreateFactory(ConstructorInfo constructor, bool byIdAware, bool byId)
    {
        if (byIdAware)
        {
            return tModel => constructor.Invoke(new object[] { tModel, byId }) as ICodeGenerator;
        }

        return tModel => constructor.Invoke(new object[] { tModel }) as ICodeGenerator;
    }

    protected Dictionary<CommonSnippets, CodeGeneratorConstruction> AvailableCodeGeneratorsCatalog()
    {
        var catalog = new Dictionary<CommonSnippets, CodeGeneratorConstruction>();

        foreach (var assembly in LoadedAssemblies)
        {
            var types = assembly.GetAvailableTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => TypeCheck.Implements<ICodeGenerator>(t));

            foreach (var type in types)
            {
                var snippetInfo = type.GetCustomAttribute<CommonSnippetAttribute>();

                if (snippetInfo != null)
                {
                    var idAware = new CommonSnippetsInfo().IsIdAware(snippetInfo.SnippetType);

                    var foundConstructor = FindConstructor(type, idAware);

                    if (foundConstructor)
                    {
                        var cons = new CodeGeneratorConstruction
                        {
                            IdAware = idAware
                        };

                        if (idAware)
                        {
                            cons.IdAgnosticCodeGenerator = t => new NullCodeGenerator();
                            cons.ByIdCodeGenerator = CreateFactory(foundConstructor, true, true);
                            cons.AllCodeGenerator = CreateFactory(foundConstructor, true, false);
                        }
                        else
                        {
                            cons.IdAgnosticCodeGenerator = CreateFactory(foundConstructor, false, false);
                            cons.ByIdCodeGenerator = cons.IdAgnosticCodeGenerator;
                            cons.AllCodeGenerator = cons.IdAgnosticCodeGenerator;
                        }

                        if (catalog.ContainsKey(snippetInfo.SnippetType))
                        {
                            //TODO: warning
                        }
                        else
                        {
                            catalog.Add(snippetInfo.SnippetType, cons);
                        }
                    }
                }
            }
        }

        return catalog;
    }
}