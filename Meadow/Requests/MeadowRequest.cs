using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Requests
{
    public abstract class MeadowRequest
    {
        public string RequestText { get; protected set; }

        public bool ReturnsValue { get; }
        
        internal bool FullTree => FullTreeReadWrite();
        
        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;

            Execution = RequestExecution.RequestTextIsNameOfRoutine;
        }
        
        
        protected virtual bool FullTreeReadWrite()
        {
            return false;
        }
        
        public RequestExecution Execution { get; protected set; }
    }
    
    
    public class MeadowRequest<TIn, TOut>:MeadowRequest
        where TOut : class, new()
    {
        public virtual TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        

        private FiledManipulationMarker<TIn> _toStorageManipulator;
        private FiledManipulationMarker<TOut> _fromStorageManipulator;

        public MeadowRequest(bool returnsValue):base(returnsValue)
        {
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


        internal IFieldMarks ToStorageMarks => _toStorageManipulator;
        internal IFieldMarks FromStorageMarks => _fromStorageManipulator;

        
    }
}