using Meadow.Requests.Common.Models;
using Meadow.Requests.Context;
using Meadow.Sql;

namespace Meadow.Requests.Common
{
    public sealed class ExistingValuesRequest<TEntity> : MeadowRequest<ValueShell<object>>
    {
        private readonly string _fieldName;

        public ExistingValuesRequest(string fieldName)
        {
            _fieldName = fieldName;
        }

        protected override void OnPreExecution(MeadowExecutionContext context)
        {
            ToStorage.Clear();

            ToStorage.Add(new
            {
                FieldName = context.FilteringTranslator.TranslateFieldName(typeof(TEntity), _fieldName),
            });
        }

        public override string RequestText => Convention<TEntity>().ExistingValuesProcedureName;
    }
}