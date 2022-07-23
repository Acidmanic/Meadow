using System.Collections.Generic;
using System.Linq;

namespace Meadow.SQLite
{
    class SqLiteInMemoryProcedures
    {
        private static SqLiteInMemoryProcedures _instance = null;
        private static readonly object Locker = new object();


        private readonly Dictionary<string, SqLiteProcedure> _procedures;
        private readonly SqLiteProcedure _doNothing;

        private SqLiteInMemoryProcedures()
        {
            _procedures = new Dictionary<string, SqLiteProcedure>();
            _doNothing = new SqLiteProcedure
            {
                Code = "",
                Name = "",
                Parameters = new Dictionary<string, string>()
            };
        }


        public static SqLiteInMemoryProcedures Instance
        {
            get
            {
                lock (Locker)
                {
                    if (_instance == null)
                    {
                        _instance = new SqLiteInMemoryProcedures();
                    }

                    return _instance;
                }
            }
        }

        public SqLiteProcedure GetProcedure(string name)
        {
            var key = SqLiteProcedure.GetKey(name);

            if (_procedures.ContainsKey(key))
            {
                return _procedures[key];
            }

            return _doNothing;
        }

        public SqLiteProcedure GetProcedureOrNull(string name)
        {
            var key = SqLiteProcedure.GetKey(name);

            if (_procedures.ContainsKey(key))
            {
                return _procedures[key];
            }

            return null;
        }

        public void AddProcedure(SqLiteProcedure procedure)
        {
            var key = procedure?.GetKey();

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_procedures.ContainsKey(key))
            {
                _procedures.Remove(key);
            }

            _procedures.Add(key, procedure);
        }

        public List<string> ListProcedures()
        {
            return new List<string>(_procedures.Values.Select(v => v.Name));
        }
    }
}