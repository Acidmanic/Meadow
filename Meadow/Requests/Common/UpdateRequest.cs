namespace Meadow.Requests.Common
{
    public sealed class UpdateRequest<TEntity> : MeadowRequest< TEntity>
    {
        public UpdateRequest(TEntity value) : base(value)
        {
        }


        public override string RequestText => Convention<TEntity>().UpdateProcedureName;
    }
}