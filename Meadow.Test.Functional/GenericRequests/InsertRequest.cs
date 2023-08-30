using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class InsertRequest<T> : MeadowRequest<T, T> where T : class, new()
    {
        public InsertRequest(T model) : base(true)
        {
            ToStorage = model;
        }

        protected override void OnFieldManipulation(IFieldInclusionMarker<T> toStorage, IFieldInclusionMarker<T> fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);

            var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
                
            toStorage.Exclude(idLeaf.GetFullName());
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<T>();

                return nc.InsertProcedureName;
            }
        }
    }
}