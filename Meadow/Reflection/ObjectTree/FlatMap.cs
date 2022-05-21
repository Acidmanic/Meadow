using System;
using System.Collections.Generic;

namespace Meadow.Reflection.ObjectTree
{
    public class FlatMap
    {
        private struct IODelegate
        {
            public readonly Func<object, object> Getter;
            public readonly Action<object,object> Setter;

            public IODelegate(Func<object, object> getter, Action<object, object> setter)
            {
                Getter = getter;
                Setter = setter;
            }
        }
        private readonly Dictionary<string, IODelegate> _flatEvaluationMap;


        public FlatMap()
        {
            _flatEvaluationMap = new Dictionary<string, IODelegate>();
        }
        
        public void Add(string fieldName, Func<object, object> getter, Action<object, object> setter)
        {
            _flatEvaluationMap.Add(fieldName, new IODelegate(getter,setter));
        }

        public object Read(string field, object rootObject)
        {
            if (_flatEvaluationMap.ContainsKey(field))
            {
                return _flatEvaluationMap[field].Getter(rootObject);
            }

            return null;
        }

        public void Write(string field, object rootObject, object value)
        {
            if (_flatEvaluationMap.ContainsKey(field))
            {
                _flatEvaluationMap[field].Setter(rootObject, value);
            }
        }

        public IEnumerable<string> FieldNames => new List<string>(_flatEvaluationMap.Keys);

        public void WalkThrough(Action<string,Func<object, object>, Action<object, object>> walkerMethod)
        {
            foreach (var keyValuePair in _flatEvaluationMap)
            {
                walkerMethod(keyValuePair.Key, keyValuePair.Value.Getter, keyValuePair.Value.Setter);
            }
        }
    }
}