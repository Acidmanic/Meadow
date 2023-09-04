using System;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.FilterResultTable)]
    public class FilterResultTableGenerator:TableCodeGeneratorBase
    {

        private readonly string _filterResultsTableName;
        
        public FilterResultTableGenerator(Type type, MeadowConfiguration configuration) :
            base(FilterResultType(type), configuration)
        {
            _filterResultsTableName = configuration.GetNameConvention(type).FilterResultsTableName;
        }

        protected override string GetTableName()
        {
            return _filterResultsTableName;
        }
    }
    
}