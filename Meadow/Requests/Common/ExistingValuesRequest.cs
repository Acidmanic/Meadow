using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public sealed class ExistingValuesRequest<TEntity> : MeadowRequest<ValueShell<object>>
    {
        public ExistingValuesRequest(string fieldName) 
        {
            RegisterTranslationTask(t =>
            {
                ToStorage.Clear();

                ToStorage.Add(new
                {
                    FieldName = t.TranslateFieldName(typeof(TEntity), fieldName, false),
                });
            });
        }

        public override string RequestText => Convention<TEntity>().ExistingValuesProcedureName;
    }
}