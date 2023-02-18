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

    [Flags]
    protected enum CodeGenerateBehavior
    {
        ById = 1,
        All = 2,
        ByIdAndAll = 3,
        None = 0
    }

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

        var constructions = BelongingCodeGenerators();


        var sb = AssembleCodeGenerators(new StringBuilder(), type, constructions);

        Title(sb, "</" + Name + ">");

        return sb.ToString();
    }


    protected virtual CodeGenerateBehavior GetGeneratingBehavior(CodeGeneratorConstruction cgc)
    {
        return CodeGenerateBehavior.ByIdAndAll;
    }


    protected virtual StringBuilder AssembleCodeGenerators(StringBuilder sb, Type entityType,
        List<CodeGeneratorConstruction> constructions)
    {
        foreach (var construction in constructions)
        {
            ICodeGenerator cg;

            if (!construction.IdAware)
            {
                cg = construction.IdAgnosticCodeGenerator(entityType);

                Append(sb, cg);
            }
            else
            {
                var behavior = GetGeneratingBehavior(construction);

                if (behavior.Is(CodeGenerateBehavior.ById))
                {
                    cg = construction.ByIdCodeGenerator(entityType);

                    Append(sb, cg);
                }

                if (behavior.Is(CodeGenerateBehavior.All))
                {
                    cg = construction.AllCodeGenerator(entityType);

                    Append(sb, cg);
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

    protected List<CodeGeneratorConstruction> BelongingCodeGenerators()
    {
        var result = new List<CodeGeneratorConstruction>();

        foreach (var assembly in LoadedAssemblies)
        {
            var types = assembly.GetAvailableTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => TypeCheck.Implements<ICodeGenerator>(t));

            foreach (var type in types)
            {
                var builtinAdapter = type.GetCustomAttribute<BuiltinMacroAdaptableAttribute>();

                if (builtinAdapter != null)
                {
                    if (builtinAdapter.BelongedMacros.Contains(Name))
                    {
                        var foundConstructor = FindConstructor(type, builtinAdapter.IdAware);

                        if (foundConstructor)
                        {
                            var cons = new CodeGeneratorConstruction
                            {
                                IdAware = builtinAdapter.IdAware
                            };

                            if (builtinAdapter.IdAware)
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

                            result.Add(cons);
                        }
                    }
                }
            }
        }

        return result;
    }
}