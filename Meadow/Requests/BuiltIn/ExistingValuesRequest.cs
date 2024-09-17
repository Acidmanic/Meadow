using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed class ExistingValuesRequest<TEntity> : MeadowRequest<FieldNameDto, ValueDto<object>>
    {
        public ExistingValuesRequest(string fieldName) : base(true)
        {
            Setup(context =>
            {
                ToStorage = new FieldNameDto(context.SqlTranslator.TranslateFieldName(typeof(TEntity), fieldName, FullTreeReadWrite()));
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