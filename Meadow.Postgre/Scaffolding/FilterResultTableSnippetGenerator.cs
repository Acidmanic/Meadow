using System;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.FilterResultTable)]
    public class FilterResultTableSnippetGenerator:TableCodeSnippetGeneratorBase
    {

        private readonly string _filterResultsTableName;
        
        public FilterResultTableSnippetGenerator(Type type, MeadowConfiguration configuration) :
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