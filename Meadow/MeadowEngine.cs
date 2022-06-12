using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Configuration.ConfigurationRequests;
using Meadow.Log;
using Meadow.Requests;

namespace Meadow
{
    public class MeadowEngine
    {
        private readonly MeadowConfiguration _configuration;
        private readonly EnhancedLogger _logger;

        public MeadowEngine(MeadowConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = new EnhancedLogger(logger);
        }

        public MeadowEngine(MeadowConfiguration configuration) : this(configuration, new NullLogger())
        {
        }

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request)
            where TOut : class, new()
        {
            if (request is ConfigurationRequest<TOut> configRequest)
            {
                var result = PerformConfigurationRequest(configRequest);

                configRequest.Result = result;

                return request;
            }

            // Run UserRequest as Procedure Request
            return new MeadowDataAccessCore().PerformRequest(request, _configuration,
                MeadowDataAccessCore.RequestExecutionType.Procedure);
        }

        private ConfigurationRequestResult PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request)
            where TOut : class, new()
        {
            try
            {
                var config = request.PreConfigure(_configuration);
                // Run Configuration Request As a Script Request
                var meadowRequest = new MeadowDataAccessCore().PerformRequest(request, config,
                    MeadowDataAccessCore.RequestExecutionType.Script);

                return new ConfigurationRequestResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ConfigurationRequestResult
                {
                    Success = false,
                    Exception = e
                };
            }
        }

        public void CreateDatabase()
        {
            PerformRequest(new CreateDatabaseRequest());

            PerformPostDatabaseCreationTasks();
        }

        private void PerformPostDatabaseCreationTasks()
        {
            var scripts = new MeadowBuiltInScripts().GenerateHistoryBasis();

            scripts.ForEach(s => PerformScript(s));
        }


        public void DropDatabase()
        {
            PerformRequest(new DropDatabaseRequest());
        }

        public bool DatabaseExists()
        {
            var exists = PerformRequest(new DatabaseExistsRequest());

            return exists.FromStorage[0].Value;
        }

        public void CreateIfNotExist()
        {
            PerformRequest(new CreateIfNotExistRequest());

            PerformPostDatabaseCreationTasks();
        }

        /// <summary>
        /// Applies all available buildup scripts
        /// </summary>
        /// <returns>A list of log reports</returns>
        public void BuildUpDatabase()
        {
            var lastExecResult = PerformRequest(new LastExecutedHistoryRequest());

            int lastAppliedOrder = -1;

            if (lastExecResult.FromStorage != null && lastExecResult.FromStorage.Count == 1)
            {
                var lastExec = lastExecResult.FromStorage[0];

                _logger.Log($"Already built up, up to {lastExec.ScriptName}:{lastExec.ScriptOrder}");

                lastAppliedOrder = lastExec.ScriptOrder;
            }

            var manager = new BuildupScriptManager(_configuration.BuildupScriptDirectory);

            if (manager.ScriptsCount == 0)
            {
                _logger.Log(
                    $@"No valid build-up scripts where found at given directory {_configuration.BuildupScriptDirectory}");
                return;
            }

            var anyApplied = false;

            for (int i = 0; i < manager.ScriptsCount; i++)
            {
                var info = manager[i];

                if (info.OrderIndex > lastAppliedOrder)
                {
                    _logger.Log($@"Applying {info.OrderIndex}, {info.Name}");
                    
                    var result = PerformScript(info);

                    if (result.Success)
                    {
                        _logger.Log($@"{info.Order}, {info.Name} has been applied successfully.");
                        
                        anyApplied = true;

                        PerformRequest(new MarkExecutionInHistoryRequest(info));
                    }
                    else
                    {
                        _logger.LogException(result.Exception, $@"Applying {info.Order}, {info.Name}");

                        _logger.Log($@"*** Buildup process FAILED at {info.Order}.***");

                        return;
                    }
                }
            }

            if (anyApplied)
            {
                _logger.Log($@"*** Buildup process SUCCEEDED ***");    
            }
            else
            {
                _logger.Log($@"*** Everything Already Up-to-date ***");
            }
            

            return;
        }


        public List<string> EnumerateProcedures()
        {
            return EnumerateDbObject(true);
        }

        public List<string> EnumerateTables()
        {
            return EnumerateDbObject(false);
        }

        private List<string> EnumerateDbObject(bool dbProcedureNotTable)
        {
            var response = dbProcedureNotTable
                ? PerformRequest(new EnumerateProceduresRequest())
                : PerformRequest(new EnumerateTablesRequest());

            var result = new List<string>();

            if (response.FromStorage != null)
            {
                result = response.FromStorage.Select(n => n.Name).ToList();
            }

            return result;
        }

        private ConfigurationRequestResult PerformScript(ScriptInfo scriptInfo)
        {
            var sqls = scriptInfo.SplitScriptIntoBatches();

            foreach (var sql in sqls)
            {
                var request = new SqlRequest(sql);

                var result = PerformConfigurationRequest(request);

                if (!result.Success)
                {
                    return new ConfigurationRequestResult
                    {
                        Exception = result.Exception,
                        Success = false
                    };
                }
            }

            return new ConfigurationRequestResult
            {
                Success = true
            };
        }
    }
}