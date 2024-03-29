namespace Meadow.Requests.Common
{
    public class ReadAllRequest<TModel> : MeadowRequest<TModel>
    {
        public override string RequestText => Convention<TModel>().SelectAllProcedureName;
    }
}