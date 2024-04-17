using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Inclusion;
using Meadow.Inclusion.Fluent;
using Meadow.Requests.Context;
using Meadow.Scaffolding.Translators;

namespace Meadow.Requests
{
    public abstract class MeadowRequest
    {
        public virtual string RequestText => GetProcedureNameFromRequestName();

        protected MeadowExecutionContext? Context { get; private set; }
        
        protected MeadowConfiguration Configuration => Context?.Configuration ?? new MeadowConfiguration();
        
        public bool ReturnsValue { get; }

        public RequestExecution Execution { get; protected set; }

        public bool Failed { get; private set; } = false;

        public Exception? FailureException { get; private set; }

        
        private readonly FiledManipulationMarker _manipulationMarker;

        
        internal IFieldInclusion InputInclusions => _manipulationMarker;

        protected IFieldInclusionMarker InputFields => _manipulationMarker;
        
        public List<object> ToStorage { get; set; }
        
        public Type?[] InTypes => ToStorage.Select(o => o?.GetType()).ToArray();
        
        public MeadowRequest(bool returnsValue, params object[] toStorage)
        {
            ReturnsValue = returnsValue;

            Execution = RequestExecution.RequestTextIsNameOfRoutine;
            
            _manipulationMarker = new FiledManipulationMarker();

            ToStorage = new List<object>();
            
            ToStorage.AddRange(toStorage);
        }

        private string GetProcedureNameFromRequestName()
        {
            var name = this.GetType().Name;

            if (name.ToLower().EndsWith("request"))
            {
                name = name.Substring(0, name.Length - "request".Length);
            }

            name = "sp" + name;

            if (Context!=null)
            {
                name = Context.LanguageTranslator.QuoteProcedureName(name);
            }

            return name;
        }
        
        
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

        internal void StartExecution(MeadowExecutionContext context)
        {
            Context = context;
            
            OnPreExecution(context);
        }


        internal void EndExecution()
        {
            OnPostExecution(Context!);
        }
        
        protected NameConvention Convention<TModel>() => Convention(typeof(TModel));

        protected NameConvention Convention(Type type)
        {
            return Configuration.GetNameConvention(type);
        }

        protected void SetToStorage(params object[] parameters)
        {
            ToStorage.Clear();
            
            ToStorage.AddRange(parameters);
        }


        protected virtual void OnPreExecution(MeadowExecutionContext context)
        {
            
        }
        
        protected virtual void OnPostExecution(MeadowExecutionContext context)
        {
            
        }
    }


    public class MeadowRequest<TOut> : MeadowRequest
    {

        public static Type OutType { get; }
        
        public static Type ReadModelType { get; }
        
        public static bool ReadsFromView { get; }
        
        public static IView? View { get; }
        
        protected NameConvention ReadModelConventions { get; }

        public List<TOut> FromStorage { get; }


        static MeadowRequest()
        {
            var type = typeof(TOut);

            OutType = type;

            ReadsFromView = false;

            ReadModelType = type;

            if (TypeCheck.InheritsFrom(typeof(ViewBase<>), type))
            {
                if (new ObjectInstantiator().BlindInstantiate(type) is IView view)
                {
                    ReadModelType = view.ModelType;

                    ReadsFromView = true;

                    View = view;
                }
            }

        }

        public MeadowRequest(params object[] toStorage) : base(true,toStorage)
        {
            FromStorage = new List<TOut>();

            ReadModelConventions = Configuration.GetNameConvention(ReadModelType);
        }

        
    }
}