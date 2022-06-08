using Meadow.Reflection.Conventions;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Requests.Common
{
    public abstract class InsertSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class, new()
    {
        protected InsertSpRequest() : base(true)
        {
        }

        protected override void OnFieldManipulation(IFieldManipulator<TModel> toStorage, IFieldManipulator<TModel> fromStorage)
        {
            toStorage.Exclude("Id");
        }

        protected override string GetRequestText()
        {
            return new NameConvention(typeof(TModel)).InsertProcedureName;
        }
    }
}