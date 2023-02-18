using System;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class BuiltinMacroCodeGeneratorRegistry
{
    private class NullCodeGenerator : ICodeGenerator
    {
        public Code Generate()
        {
            return new Code
            {
                Name = "NULL",
                Text = ""
            };
        }
    }

    private static ICodeGenerator NullCodeGeneratorFactory(Type entity, bool byId)
    {
        return new NullCodeGenerator();
    }

    internal static Func<Type, bool, ICodeGenerator> DeleteCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> EventStreamCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> InsertCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> ReadCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> ReadSequenceCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> SaveCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> TableCodeGenerator { get; private set; } = NullCodeGeneratorFactory;
    internal static Func<Type, bool, ICodeGenerator> UpdateCodeGenerator { get; private set; } = NullCodeGeneratorFactory;


    [Flags]
    public enum CodeGenerators
    {
        Table = 1,
        Insert = 2,
        Read = 4,
        ReadSequence = 8,
        Delete = 16,
        Update = 32,
        Save = 64,
        EventStream = 128
    }


    private static bool Is(CodeGenerators codeGenerator, CodeGenerators specific)
    {
        return ((codeGenerator & specific) == codeGenerator);
    }
    
    public void Register(CodeGenerators functionality, Func<Type, bool, ICodeGenerator> factory)
    {
        if (Is(functionality,CodeGenerators.Table))
        {
            TableCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.Insert))
        {
            InsertCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.Read))
        {
            ReadCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.ReadSequence))
        {
            ReadSequenceCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.Delete))
        {
            DeleteCodeGenerator= factory;
        }
        if (Is(functionality,CodeGenerators.Update))
        {
            UpdateCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.Save))
        {
            SaveCodeGenerator = factory;
        }
        if (Is(functionality,CodeGenerators.EventStream))
        {
            EventStreamCodeGenerator = factory;
        }
    }
}