using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public sealed  class RangeRequest<TEntity>:MeadowRequest<FieldRange>
    {
        public RangeRequest(string fieldName) 
        {
            RegisterTranslationTask(t =>
            {
                SetToStorage(new
                {
                    FieldName = t.TranslateFieldName(typeof(TEntity),fieldName,false)
                });
            });
        }

        public override string RequestText => Convention<TEntity>().RangeProcedureName;
    }
}