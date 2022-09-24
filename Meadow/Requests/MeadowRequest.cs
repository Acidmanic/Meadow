using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.FieldInclusion;

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

        internal void InitializeBeforeExecution()
        {
            RequestText = GetProcedureNameFromRequestName();

            

            _toStorageManipulator = new FiledManipulationMarker<TIn>();
            _fromStorageManipulator = new FiledManipulationMarker<TOut>();

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

            name =  "sp" + name;

            if (QuoteProcedureName())
            {
                name = $"\"{name}\"";
            }

            return name;
        }

        protected virtual bool QuoteProcedureName()
        {
            return false;
        }
        
        protected virtual void OnFieldManipulation(IFieldInclusionMarker<TIn> toStorage,
            IFieldInclusionMarker<TOut> fromStorage)
        {
        }


        internal IFieldInclusion<TIn> ToStorageInclusion => _toStorageManipulator;
        internal IFieldInclusion<TOut> FromStorageInclusion => _fromStorageManipulator;

        
    }
}