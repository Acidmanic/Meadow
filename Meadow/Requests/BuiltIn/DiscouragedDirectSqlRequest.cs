namespace Meadow.Requests.BuiltIn;

public class DiscouragedDirectSqlRequest<TReturn>:MeadowRequest<MeadowVoid,TReturn> where TReturn : class
{
    private readonly string _sql;
    
    public DiscouragedDirectSqlRequest(string sql) : base(true)
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
public class DiscouragedDirectSqlRequest:MeadowRequest<MeadowVoid,MeadowVoid>
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