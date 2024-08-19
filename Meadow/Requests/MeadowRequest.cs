using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Configuration;

namespace Meadow.Requests
{
    public abstract class MeadowRequest
    {
        public virtual string RequestText { get; protected set; }

        protected MeadowConfiguration Configuration { get; private set; }
        
        public bool ReturnsValue { get; }

        /// <summary>
        /// Engine Can Suggest the request to consider itself a full-tree access request. This suggestion is made
        /// by client code using Meadow. This way a request's full-tree access behavior can be determined out side of
        /// the request. For example, you can Update the procedure's sql to fullTree access, and you dont have access
        /// to the request's code, and the only thing you need to read the full-tree data, is to tell the request to be
        /// a full-tree request.  
        /// </summary>
        private bool _suggestedFullTreeAccess = false;
        
        
        internal void SuggestFullTreeReadWrite(bool fullTree)
        {
            _suggestedFullTreeAccess = fullTree;
        }
        /// <summary>
        /// This is where a specific implementation of a request is able to discard the suggested full-tree behavior,
        /// and override it the way that implementation see's suitable.
        /// </summary>
        /// <returns>The Request implementation's decision about full-tree behavior. Default is
        /// to return the value of SuggestedFullTreeAccess which it's default is false.
        /// </returns>
        protected virtual bool FullTreeReadWrite()
        {
            return _suggestedFullTreeAccess;
        }
        /// <summary>
        /// This is the outlet that meadow Engine/DataAccessCore would check to see If the request should be treated as
        /// full-tree or not.  
        /// </summary>
        internal bool FullTree => FullTreeReadWrite();

        private FiledManipulationMarker _toStorageManipulator = new FiledManipulationMarker();
        
        private FiledManipulationMarker _fromStorageManipulator = new FiledManipulationMarker();
        
        private Action<RequestContext> _setupActions = c => { };

        internal IFieldInclusion ToStorageInclusion => _toStorageManipulator;
        
        internal IFieldInclusion FromStorageInclusion => _fromStorageManipulator;
        
        
        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;

            Execution = RequestExecution.RequestTextIsNameOfRoutine;
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

        internal void SetContext(RequestContext context)
        {
            _setupActions(context);

            Configuration = context.Configuration;
        }
        
        protected virtual bool QuoteProcedureName()
        {
            return false;
        }

        protected virtual void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {
        }
        
        protected string GetProcedureNameFromRequestName()
        {
            var name = this.GetType().Name;

            if (name.ToLower().EndsWith("request"))
            {
                name = name.Substring(0, name.Length - "request".Length);
            }

            name = "sp" + name;

            if (QuoteProcedureName())
            {
                name = $"\"{name}\"";
            }

            return name;
        }
        
        protected void Setup(Action<RequestContext> setup) => _setupActions = setup;
        
        internal void InitializeBeforeExecution()
        {
            RequestText = GetProcedureNameFromRequestName();

            _toStorageManipulator.Clear();

            OnFieldManipulation(_toStorageManipulator, _fromStorageManipulator);
        }
    }


    public class MeadowRequest<TIn, TOut> : MeadowRequest where TOut : class
    {
        public virtual TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public MeadowRequest(bool returnsValue) : base(returnsValue)
        {
            FromStorage = new List<TOut>();
        }
        
    }
}