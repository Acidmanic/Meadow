using System;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.DataAccessResolving;

public class NullDataAccessServiceResolver:IDataAccessServiceResolver
{

    public ISqlTranslator SqlTranslator => ISqlTranslator.Null;

    public IDbTypeNameMapper DbTypeNameMapper => IDbTypeNameMapper.Null;

    public ISnippet? InstantiateSnippet(CommonSnippets commonSnippets) => null;

    public T GetService<T>() where T : class =>  throw new Exception($"Unable to Instantiate {typeof(T).Name}");

}