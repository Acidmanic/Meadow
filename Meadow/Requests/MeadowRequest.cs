using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Requests;

namespace Meadow
{
    public class MeadowRequest<TIn, TOut>
        where TOut : class, new()
    {
        public TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public string RequestText { get; protected set; }

        public bool ReturnsValue { get; }


        private List<string> _excludingInputFields;

        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;

            FromStorage = new List<TOut>();

            _excludingInputFields = new List<string>();
        }

        internal void InitializeBeforeExecution()
        {
            RequestText = GetRequestText();

            _excludingInputFields.Clear();

            var exclusionMarker = new FieldExclusionMarker<TIn>();

            OnExclusion(exclusionMarker);

            _excludingInputFields = exclusionMarker.ExcludedNames();
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

        protected virtual void OnExclusion(FieldExclusionMarker<TIn> exclusionMarker)
        {
        }

        internal bool IsIncluded(string fieldName)
        {
            return !_excludingInputFields.Contains(fieldName);
        }
    }
}