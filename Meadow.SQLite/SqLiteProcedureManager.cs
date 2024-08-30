using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meadow.SQLite.Exceptions;
using Meadow.SQLite.ProcedureProcessing;
using Meadow.Utility;
using Newtonsoft.Json;

namespace Meadow.SQLite
{
    internal class SqLiteProcedureManager
    {
        private static readonly object InstantiationLock = new object();

        private static readonly Dictionary<string, SqLiteProcedureManager> InstancesByFile =
            new Dictionary<string, SqLiteProcedureManager>();


        private readonly object _operationLock = new object();
        private readonly Dictionary<string, SqLiteProcedure> _procedures;
        private readonly string _filePath;
        private readonly SqLiteProcedure _doNothing;


        private SqLiteProcedureManager(string filePath)
        {
            _procedures = new Dictionary<string, SqLiteProcedure>();
            _doNothing = new SqLiteProcedure
            {
                Code = "",
                Name = "",
                ParameterTypesByParameterName = new Dictionary<string, string>()
            };
            _filePath = filePath;
        }

        private static string GetProceduresFile(string connectionString)
        {
            var conInfo = new ConnectionStringParser().Parse(connectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"] + ".json";

                return new FileInfo(filename).FullName;
            }

            return "procedures.json";
        }


        public static SqLiteProcedureManager Connect(string connectionString)
        {
            lock (InstantiationLock)
            {
                var proceduresFile = GetProceduresFile(connectionString);

                if (!InstancesByFile.ContainsKey(proceduresFile))
                {
                    var instance = new SqLiteProcedureManager(proceduresFile);

                    InstancesByFile.Add(proceduresFile, instance);
                }

                InstancesByFile[proceduresFile].LoadExisting();

                return InstancesByFile[proceduresFile];
            }
        }

        public SqLiteProcedure GetProcedure(string name)
        {
            lock (_operationLock)
            {
                var key = SqLiteProcedure.GetKey(name);

                if (_procedures.ContainsKey(key))
                {
                    return _procedures[key];
                }

                return _doNothing;
            }
        }

        public SqLiteProcedure GetProcedureOrNull(string name)
        {
            lock (_operationLock)
            {
                var key = SqLiteProcedure.GetKey(name);

                if (_procedures.ContainsKey(key))
                {
                    return _procedures[key];
                }

                return null;
            }
        }


        public List<string> ListProcedures()
        {
            lock (_operationLock)
            {
                return new List<string>(_procedures.Values.Select(v => v.Name));
            }
        }

        public void DropStoredRoutines()
        {
            lock (_operationLock)
            {
                try
                {
                    File.Delete(_filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                _procedures.Clear();
            }
        }

        private void LoadExisting()
        {
            lock (_operationLock)
            {
                if (File.Exists(_filePath))
                {
                    _procedures.Clear();

                    try
                    {
                        var json = File.ReadAllText(_filePath);

                        var existingProcedures =
                            JsonConvert.DeserializeObject<Dictionary<string, SqLiteProcedure>>(json)
                            ?? new Dictionary<string, SqLiteProcedure>();

                        foreach (var keyValuePair in existingProcedures)
                        {
                            _procedures.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        private void SaveToFile()
        {
            var json = JsonConvert.SerializeObject(this._procedures);

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            File.WriteAllText(_filePath, json);
        }

        private void CreateProcedure(SqLiteProcedure procedure)
        {
            var key = procedure?.GetKey();

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_procedures.ContainsKey(key))
            {
                throw new SqLiteScriptException($"Can not create procedure {procedure.Name}, " +
                                                $"Since a procedure with this name already exists.");
            }

            _procedures.Add(key, procedure);

            SaveToFile();
        }

        private void CreateIfNotExists(SqLiteProcedure procedure)
        {
            var key = procedure?.GetKey();

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_procedures.ContainsKey(key))
            {
                return;
            }

            _procedures.Add(key, procedure);

            SaveToFile();
        }

        private void DropProcedure(SqLiteProcedure procedure)
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
            else
            {
                throw new SqLiteScriptException($"Can not drop procedure {procedure.Name}, " +
                                                $"The procedure does not exists.");
            }

            SaveToFile();
        }

        private void DropIfExists(SqLiteProcedure procedure)
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

            SaveToFile();
        }

        private void AlterProcedure(SqLiteProcedure procedure)
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
            else
            {
                throw new SqLiteScriptException($"Can not alter procedure {procedure.Name}, " +
                                                $"The procedure does not exists.");
            }

            _procedures.Add(key, procedure);

            SaveToFile();
        }

        private void CreateOrAlter(SqLiteProcedure procedure)
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

        public void PerformProcedureCreation(SqLiteProcedure procedure)
        {
            lock (_operationLock)
            {
                var creationHandlers = new Dictionary<string, Action>
                {
                    { DbObjectCreation.Alter.ToSql(), () => AlterProcedure(procedure) },
                    { DbObjectCreation.Create.ToSql(), () => CreateProcedure(procedure) },
                    { DbObjectCreation.Drop.ToSql(), () => DropProcedure(procedure) },
                    { DbObjectCreation.DropIfExists.ToSql(), () => DropIfExists(procedure) },
                    { DbObjectCreation.CreateIfNotExists.ToSql(), () => CreateIfNotExists(procedure) },
                    { DbObjectCreation.CreateOrAlter.ToSql(), () => CreateOrAlter(procedure) },
                };

                var key = procedure.Creation.ToSql();

                if (creationHandlers.ContainsKey(key))
                {
                    creationHandlers[key]();
                }
                else
                {
                    throw new SqLiteScriptException($"Unknown operation: {procedure.Creation.ToSql()}");
                }
            }
        }
    }
}