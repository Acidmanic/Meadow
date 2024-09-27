using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Extensions;

namespace Meadow.Requests.BuiltIn;

public class DiscouragedDirectSqlRequest<TReturn> : MeadowRequest<MeadowVoid, TReturn> where TReturn : class
{
    private string _executingSql;

    public DiscouragedDirectSqlRequest(string sql, object? parameters = null) : base(true)
    {
        _executingSql = sql;

        if (parameters is { } p)
        {
            Setup(context => { _executingSql = Inject(context, sql, p); });
        }

        Execution = RequestExecution.RequestTextIsExecutable;
    }

    private string Inject(RequestContext context, string sql, object parameters)
    {
        var ev = new ObjectEvaluator(parameters);

        var flatten = ev.ToStandardFlatData(o => o.ExcludeNulls().FullTree().UseAlternativeTypes());

        var fullTreeMap = context.Configuration.GetFullTreeMap(parameters.GetType());
        
        foreach (var dp in flatten)
        {
            var name = ev.Map.FieldKeyByAddress(dp.Identifier).Headless().ToString();

            var value = context.SqlTranslator.TranslateValue(dp.Value);

            sql = sql.Replace("{" + name + "}", value);

            var parameterName = fullTreeMap.GetColumnName(name);

            if (parameterName)
            {
                sql = sql.Replace(parameterName, value);
            }
        }

        return sql;
    }

    public override string RequestText
    {
        get => _executingSql;
        protected set { }
    }
}

public class DiscouragedDirectSqlRequest : MeadowRequest<MeadowVoid, MeadowVoid>
{
    private readonly string _sql;

    public DiscouragedDirectSqlRequest(string sql) : base(false)
    {
        _sql = sql;
        Execution = RequestExecution.RequestTextIsExecutable;
    }

    public override string RequestText
    {
        get => _sql;
        protected set { }
    }
}