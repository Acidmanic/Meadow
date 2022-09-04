using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Requests
{
    public abstract class MeadowRequest
    {
        public virtual string RequestText { get; protected set; }

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

        public bool Failed { get; private set; } = false;
        
        public Exception FailureException { get; private set; }

        public void SetFailure(Exception exception)
        {
            Failed = true;

            FailureException = exception;
        }

        public void SetFailure(string reason)
        {
            Failed = true;
            
            FailureException = new Exception(reason);
        }
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
            RequestText = GetProcedureNameFromRequestName();

            

            _toStorageManipulator = new FiledManipulationMarker<TIn>(dataOwnerNameProvider,FullTree);
            _fromStorageManipulator = new FiledManipulationMarker<TOut>(dataOwnerNameProvider,FullTree);

            _toStorageManipulator.Clear();
            
            OnFieldManipulation(_toStorageManipulator, _fromStorageManipulator);
        }

        protected string GetProcedureNameFromRequestName()
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


        internal IFieldMarks<TIn> ToStorageMarks => _toStorageManipulator;
        internal IFieldMarks<TOut> FromStorageMarks => _fromStorageManipulator;

        
    }
}