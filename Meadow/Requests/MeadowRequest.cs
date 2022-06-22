using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Requests
{
    public class MeadowRequest<TIn, TOut>
        where TOut : class, new()
    {
        public virtual TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public string RequestText { get; protected set; }

        public bool ReturnsValue { get; }

        private FiledManipulationMarker<TIn> _toStorageManipulator;
        private FiledManipulationMarker<TOut> _fromStorageManipulator;

        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;

            FromStorage = new List<TOut>();
        }

        internal void InitializeBeforeExecution(IDataOwnerNameProvider dataOwnerNameProvider)
        {
            RequestText = GetRequestText();

            

            _toStorageManipulator = new FiledManipulationMarker<TIn>(dataOwnerNameProvider);
            _fromStorageManipulator = new FiledManipulationMarker<TOut>(dataOwnerNameProvider);

            _toStorageManipulator.Clear();
            
            OnFieldManipulation(_toStorageManipulator, _fromStorageManipulator);
        }

        protected virtual string GetRequestText()
        {
            var name = this.GetType().Name;

            if (name.ToLower().EndsWith("request"))
            {
                name = name.Substring(0, name.Length - "request".Length);
            }

            return "sp" + name;
        }

        protected virtual void OnFieldManipulation(IFieldManipulator<TIn> toStorage,
            IFieldManipulator<TOut> fromStorage)
        {
        }

        protected virtual bool FullTreeReadWrite()
        {
            return false;
        }

        internal IFieldMarks ToStorageMarks => _toStorageManipulator;
        internal IFieldMarks FromStorageMarks => _fromStorageManipulator;

        internal bool FullTree => FullTreeReadWrite();
    }
}