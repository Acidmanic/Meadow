using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public class InsertRequest<TModel> : MeadowRequest<TModel>
    {
        public InsertRequest(params object[] toStorage) : base(toStorage)
        {
            var entityType = typeof(TModel);
            
            var evaluator = new ObjectEvaluator(entityType);
            
            var idKey = evaluator.FindIdField();

            if (idKey != null)
            {
                InputFields.Exclude(idKey);   
            }
        }

        public override string RequestText => Configuration.GetNameConvention<TModel>().InsertProcedureName;
        

    }
}