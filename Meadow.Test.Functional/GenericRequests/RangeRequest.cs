using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed  class RangeRequest<TEntity>:MeadowRequest<FieldNameShell,FieldRange>
    {
        public RangeRequest(string fieldName) : base(true)
        {
            Setup(context =>
            {
                ToStorage = new FieldNameShell
                {
                    FieldName = context.SqlTranslator.TranslateFieldName(typeof(TEntity),fieldName,FullTreeReadWrite())
                };
            });
        }
        
        public override string RequestText
        {
            get => Configuration.GetNameConvention(typeof(TEntity)).RangeProcedureName;
            protected set
            {
                
            }
        }
    }
}