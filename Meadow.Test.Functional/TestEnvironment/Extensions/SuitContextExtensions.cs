using Meadow.DataAccessResolving;
using Meadow.Extensions;
using Meadow.Sql.Extensions;

namespace Meadow.Test.Functional.TestEnvironment.Extensions;

public static  class SuitContextExtensions
{
    
    public static string TranslateSelectAll<TModel>(this ISuitContext context, bool fullTree = false)
    {

        var configuration = context.MeadowConfiguration;

        var resolver = new DataAccessServiceResolver(configuration);

        var tr = resolver.SqlTranslator;
        
        var semi = tr.UsesSemicolon ? ";" : string.Empty;

        var nc = configuration.GetNameConvention(typeof(TModel));

        var source = fullTree ? nc.FullTreeViewName : nc.TableName;

        source = tr.GetQuoters().QuoteTableName(source);

        return $"SELECT * FROM {source}{semi}";
    }
}