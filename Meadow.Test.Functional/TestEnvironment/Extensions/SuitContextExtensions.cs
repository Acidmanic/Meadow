using System.Diagnostics.CodeAnalysis;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.DataAccessResolving;
using Meadow.Extensions;
using Meadow.Sql.Extensions;

namespace Meadow.Test.Functional.TestEnvironment.Extensions;

public static class SuitContextExtensions
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

    public static string TranslateInsert<TModel>(this ISuitContext context, TModel model) where TModel : class
    {
        var configuration = context.MeadowConfiguration;

        var resolver = new DataAccessServiceResolver(configuration);

        var sqlTranslator = resolver.SqlTranslator;
        var valueTranslator = resolver.ValueTranslator;

        var semi = sqlTranslator.UsesSemicolon ? ";" : string.Empty;

        var type = typeof(TModel);

        var nc = configuration.GetNameConvention(type);

        var qt = sqlTranslator.GetQuoters();
        
        var source = qt.QuoteTableName(nc.TableName);
        
        var ev = new ObjectEvaluator(model);

        var flatten = ev.ToStandardFlatData(o => o.ExcludeNulls().FullTree().UseAlternativeTypes());

        var fullTreeMap = configuration.GetFullTreeMap(type);

        var columns = new List<string>();
        var values = new List<string>();

        
        foreach (var dp in flatten)
        {
            var name = ev.Map.FieldKeyByAddress(dp.Identifier).Headless().ToString();

            var parameterName = fullTreeMap.GetColumnName(name);

            if (parameterName)
            {
                var value = valueTranslator.Translate(dp.Value);
                
                columns.Add(qt.QuoteColumnName(parameterName));
                
                values.Add(value);
            }
        }

        return $"INSERT INTO {source} ({string.Join(',',columns)}) VALUES ({string.Join(',',values)}){semi}";
    }
}