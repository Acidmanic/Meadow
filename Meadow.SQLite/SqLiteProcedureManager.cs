using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meadow.SQLite.ProcedureProcessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Meadow.SQLite
{
    class SqLiteProcedureManager
    {
        private static SqLiteProcedureManager _instance = null;
        private static readonly object Locker = new object();


        private readonly Dictionary<string, SqLiteProcedure> _procedures;
        private  string _filePath;
        private  bool _isAssignedToFile;
        
        private readonly SqLiteProcedure _doNothing;

        private SqLiteProcedureManager()
        {
            _procedures = new Dictionary<string, SqLiteProcedure>();
            _doNothing = new SqLiteProcedure
            {
                Code = "",
                Name = "",
                Parameters = new Dictionary<string, string>()
            };
        }


        public static SqLiteProcedureManager Instance
        {
            get
            {
                lock (Locker)
                {
                    if (_instance == null)
                    {
                        _instance = new SqLiteProcedureManager();
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
            
            SaveToFile();
        }

        public List<string> ListProcedures()
        {
            return new List<string>(_procedures.Values.Select(v => v.Name));
        }


        public void AssignDatabase(string file)
        {
            var proceduresFile = file + ".json";

            _filePath = proceduresFile;

            _isAssignedToFile = true;
            
            if (File.Exists(proceduresFile))
            {
                _procedures.Clear();
                
                try
                {
                    var json = File.ReadAllText(proceduresFile);

                    var procs = JsonConvert.DeserializeObject < Dictionary<string, SqLiteProcedure>>(json);
                    
                    foreach (var keyValuePair in procs)
                    {
                        _procedures.Add(keyValuePair.Key,keyValuePair.Value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }                
            }
            else
            {
                SaveToFile();
            }
            
            
        }

        private void SaveToFile()
        {
            if (_isAssignedToFile)
            {
                var json = JsonConvert.SerializeObject(this._procedures);

                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
                
                File.WriteAllText(_filePath,json);
            }
        }
    }
}