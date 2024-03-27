using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public abstract class InsertSpRequest<TModel> : MeadowRequest<TModel>
    {
        protected InsertSpRequest(params object[] toStorage) : base(toStorage)
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