using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;

namespace Meadow.Configuration
{
    public class MeadowConfiguration : MeadowConfigurationModel
    {
        private readonly Dictionary<Type,FilterQuery> _filters = new ();
        private ITransliterationService _transliterationService = new DefaultTransliterationService();

        public List<Assembly> MacroContainingAssemblies { get; set; } = new List<Assembly>();

        public IDataOwnerNameProvider TableNameProvider { get; set; } = new PluralDataOwnerNameProvider();

        public List<ICast> ExternalTypeCasts { get; set; } = new List<ICast>();


        public IReadOnlyDictionary<Type, FilterQuery> Filters => _filters;

        public ITransliterationService TransliterationService => _transliterationService;

        public MeadowConfiguration AddFilter<TEntity>(Action<FilterQueryBuilder<TEntity>> builder)
        {
            var filterBuilder = new FilterQueryBuilder<TEntity>();

            builder(filterBuilder);

            _filters[typeof(TEntity)] = filterBuilder.Build();

            return this;
        }

        public MeadowConfiguration SetTransliterationService(ITransliterationService transliterationService)
        {
            _transliterationService = transliterationService;

            return this;
        }
        
        public MeadowConfiguration SetTransliterationService(Func<string,string> transliterationService)
        {
            _transliterationService = new FuncAdapterTransliterator(transliterationService);

            return this;
        }
    }
}