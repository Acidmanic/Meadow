namespace Meadow.Requests.Common
{
    public class SaveRequest<TModel> : MeadowRequest<TModel>
    {
        public SaveRequest(TModel value) : base(value)
        {
        }
        
        public override string RequestText => Convention<TModel>().SaveProcedureName;
    }
}