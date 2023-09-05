using System;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FilterResultTable)]
    public class FilterResultTableSnippetGenerator:TableScriptSnippetGeneratorBase
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