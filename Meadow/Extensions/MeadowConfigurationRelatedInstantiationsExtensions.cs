using System;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.RelationalStandardMapping;

namespace Meadow.Extensions;

public static class MeadowConfigurationRelatedInstantiationsExtensions
{
    public static IRelationalIdentifierToStandardFieldMapper GetRelationalStandardMapper(
        this MeadowConfiguration configuration)
    {
        return new FlatRelationalToStandardMapper(configuration.TableNameProvider);
    }

    public static NameConvention GetNameConvention(this MeadowConfiguration configuration, Type type)
    {
        return new NameConvention(type, configuration.TableNameProvider);
    }

    public static NameConvention GetNameConvention<TEntity>(this MeadowConfiguration configuration)
    {
        return new NameConvention(typeof(TEntity), configuration.TableNameProvider);
    }

    public static FullTreeMap GetFullTreeMap(this MeadowConfiguration configuration, Type type)
    {
        return new FullTreeMap(type, configuration.DatabaseFieldNameDelimiter, configuration.TableNameProvider);
    }

    public static FullTreeMap GetFullTreeMap<T>(this MeadowConfiguration configuration)
    {
        return new FullTreeMap<T>(configuration.DatabaseFieldNameDelimiter, configuration.TableNameProvider);
    }
    
}