using System;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FilterResultTable)]
    public class FilterResultTableGenerator:CreateTableScriptGeneratorBase
    {

        private readonly string _filterResultsTableName;
        
        public FilterResultTableGenerator(Type type, MeadowConfiguration configuration) :
            base(FilterResultType(type), configuration,true)
        {
            _filterResultsTableName = configuration.GetNameConvention(type).FilterResultsTableName;
        }

        protected override string GetTableName(ProcessedType processedType)
        {
            return _filterResultsTableName;
        }
    }
    
}