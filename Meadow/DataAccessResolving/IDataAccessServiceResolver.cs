using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.DataAccessResolving;

public interface IDataAccessServiceResolver
{
    public static readonly IDataAccessServiceResolver Null = new NullDataAccessServiceResolver();
    
    ISqlTranslator SqlTranslator { get; }

    IDbTypeNameMapper DbTypeNameMapper { get; }

    IValueTranslator ValueTranslator { get; }
    
    ISnippet? InstantiateSnippet(CommonSnippets commonSnippets);

    T GetService<T>() where T : class;
}