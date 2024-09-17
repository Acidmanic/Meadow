using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed  class RangeRequest<TEntity,TField>:MeadowRequest<FieldNameDto,FieldRangeDto<TField>>
    {
        public RangeRequest(Expression<Func<TEntity,TField>> selector) : base(true)
        {
            Setup(context =>
            {
                var fieldName = MemberOwnerUtilities.GetKey(selector).TerminalSegment().Name;
                
                ToStorage = new FieldNameDto(context.SqlTranslator.TranslateFieldName(typeof(TEntity),fieldName,FullTreeReadWrite()));
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