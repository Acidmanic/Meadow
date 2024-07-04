using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Requests.Common
{
    public abstract class InsertSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class, new()
    {
        protected InsertSpRequest() : base(true)
        {
        }

        protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {

            var entityType = typeof(TModel);
            
            var evaluator = new ObjectEvaluator(entityType);
            
            var allLeaves = evaluator.RootNode.EnumerateLeavesBelow();
            
            var idLeaf = TypeIdentity.FindIdentityLeaf(entityType);

            FieldKey idKey = GetKey(idLeaf, evaluator, allLeaves);

            if (idKey != null)
            {
                toStorage.Exclude(idKey);   
            }
        }
        
        private FieldKey GetKey(AccessNode idLeaf, ObjectEvaluator evaluator, List<AccessNode> allLeaves)
        {
            if (idLeaf == null)
            {
                return null;
            }

            var fullName = idLeaf.GetFullName();

            var corresponding  = allLeaves.FirstOrDefault(l => l.GetFullName() == fullName);

            if (corresponding == null)
            {
                return null;
            }

            return evaluator.Map.FieldKeyByNode(corresponding);

        }

    }
}