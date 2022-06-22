using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Exceptions;

namespace Meadow.Requests.Common
{
    public abstract class ByIdRequestBase<TModel, TId, TOut> : MeadowRequest<object, TOut>
        where TModel : class, new() where TOut : class, new()
    {
        private readonly bool _fullTree;
        private readonly string _idFieldName;
        private readonly Type _idFieldType;
        private TId _id;

        protected ByIdRequestBase(bool returnsValue, bool fullTree) : base(returnsValue)
        {
            _fullTree = fullTree;

            var modelType = typeof(TModel);

            var node = ObjectStructure.CreateStructure(modelType, false);

            var idField = node.GetDirectLeaves().FirstOrDefault(l => l.IsUnique);

            if (idField == null)
            {
                throw new ModelMustHaveIdentifierException();
            }

            _idFieldName = idField.Name;

            _idFieldType = idField.Type;
        }

        public TId Id
        {
            get => _id;
            set
            {
                _id = value;

                ToStorage = new ModelBuilder("IdShell")
                    .AddProperty(_idFieldName, _idFieldType)
                    .BuildObject();
            }
        }
    }
}