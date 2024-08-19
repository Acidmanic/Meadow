using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class ExistingValuesRequest<TEntity> : MeadowRequest<FieldNameShell, ValueShell<object>>
    {
        public ExistingValuesRequest(string fieldName) : base(true)
        {
            Setup(context =>
            {
                ToStorage = new FieldNameShell
                {
                    FieldName = context.Translator.TranslateFieldName(typeof(TEntity),fieldName,FullTreeReadWrite())
                };
            });
        }

        public override string RequestText
        {
            get => Configuration.GetNameConvention(typeof(TEntity)).ExistingValuesProcedureName;
            protected set
            {
                
            }
        }
    }
}