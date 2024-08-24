using System;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.DataAccessResolving;

public class DataAccessServiceResolver
{
    private readonly MeadowConfiguration _configuration;


    public DataAccessServiceResolver(MeadowConfiguration configuration)
    {
        _configuration = configuration;
    }


    public ISqlTranslator SqlTranslator => Produce<ISqlTranslator>();

    public IDbTypeNameMapper DbTypeNameMapper => Produce<IDbTypeNameMapper>();


    public ISnippet? InstantiateSnippet(CommonSnippets commonSnippets)
    {
        var snippetType = Find<ISnippet>(a => a is CommonSnippetAttribute csa && csa.SnippetType == commonSnippets);
        
        if (snippetType is { } type)
        {
            return ConstructOrDefault(type) as ISnippet;
        }

        return null;
    }

    private T Produce<T>() where T : class
    {
        var foundType = Find<T>();

        var produced = Resolve(foundType);

        if (produced as T is { } p) return p;

        throw new Exception($"Unable to Instantiate {typeof(T).Name} using the type: {foundType.FullName}");
    }


    private object? Resolve(Type type)
    {
        var byMeadowConfiguration = ConstructOrDefault(type, _configuration);

        if (byMeadowConfiguration is { } resolved) return resolved;

        return ConstructOrDefault(type);
    }


    private object? ConstructOrDefault(Type type, params object[] parameters)
    {
        var parameterTypes = parameters.Select(p => p.GetType()).ToArray();

        var constructor = type.GetConstructors()
            .FirstOrDefault(constructorInfo => IsMatch(constructorInfo, parameterTypes));

        if (constructor is { } c)
        {
            try
            {
                return c.Invoke(parameters);
            }
            catch
            {
                /* Ignore */
            }
        }

        return null;
    }


    private bool IsMatch(ConstructorInfo constructorInfo, params Type[] parameterTypes)
    {
        var constructorTypes = constructorInfo.GetParameters();

        if (parameterTypes.Length != constructorTypes.Length) return false;

        for (int i = 0; i < parameterTypes.Length; i++)
        {
            if (parameterTypes[i] != constructorTypes[i].ParameterType) return false;
        }

        return true;
    }


    private Type Find<T>(Func<Attribute, bool>? attributeFilter = null)
    {
        Func<Attribute, bool> aFilter = _ => true;

        if (attributeFilter is { } filter) aFilter = filter;

        if (MeadowEngine.DataAccessAssembly() is { } assembly)
        {
            var typeToFind = typeof(T);

            var foundType = assembly
                .GetAvailableTypes()
                .FirstOrDefault(t =>
                    !t.IsAbstract && !t.IsInterface &&
                    TypeCheck.InheritsFrom(typeToFind, t)
                    && t.GetCustomAttributes().Any(aFilter));

            if (foundType is { } implementationType)
            {
                return implementationType;
            }
        }

        throw new Exception("You have to select your DataAccess using MeadowEngine.UseDataAccess() " +
                            "before being able to use DataAccess implementations");
    }
}