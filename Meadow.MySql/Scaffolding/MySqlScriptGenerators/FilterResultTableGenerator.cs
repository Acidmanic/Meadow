using System;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FilterResultTable)]
    public class FilterResultTableGenerator:TableScriptGeneratorBase
    {

        private readonly string _filterResultsTableName;
        
        public FilterResultTableGenerator(Type type, MeadowConfiguration configuration) :
            base(FilterResultType(type), configuration)
        {
            _filterResultsTableName = configuration.GetNameConvention(type).FilterResultsTableName;
        }

        protected override string GetTableName(ProcessedType process)
        {
            return _filterResultsTableName;
        }
    }
    
}