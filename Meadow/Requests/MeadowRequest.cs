using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Reflection;
using Meadow.Requests;
using Meadow.Requests.FieldManipulation;

namespace Meadow
{
    public class MeadowRequest<TIn, TOut>
        where TOut : class, new()
    {
        public TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public string RequestText { get; protected set; }

        public bool ReturnsValue { get; }

        private readonly FiledManipulationMarker<TIn> _toStorageManipulator;
        private readonly FiledManipulationMarker<TOut> _fromStorageManipulator;

        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;

            FromStorage = new List<TOut>();

            _toStorageManipulator = new FiledManipulationMarker<TIn>();
            _fromStorageManipulator = new FiledManipulationMarker<TOut>();
        }

        internal void InitializeBeforeExecution()
        {
            RequestText = GetRequestText();

            _toStorageManipulator.Clear();

            var exclusionMarker = new FiledManipulationMarker<TIn>();

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