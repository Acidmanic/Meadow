using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Configuration
{
    public class MeadowConfiguration : MeadowConfigurationModel
    {
        private readonly Dictionary<Type,FilterQuery> _filters = new ();

        public List<Assembly> MacroContainingAssemblies { get; set; } = new List<Assembly>();

        public IDataOwnerNameProvider TableNameProvider { get; set; } = new PluralDataOwnerNameProvider();

        public List<ICast> ExternalTypeCasts { get; set; } = new List<ICast>();


        public IReadOnlyDictionary<Type, FilterQuery> Filters => _filters;

        public MeadowConfiguration AddFilter<TEntity>(Action<FilterQueryBuilder<TEntity>> builder)
        {
            var filterBuilder = new FilterQueryBuilder<TEntity>();

            builder(filterBuilder);

            _filters[typeof(TEntity)] = filterBuilder.Build();

            return this;
        }
    }
}