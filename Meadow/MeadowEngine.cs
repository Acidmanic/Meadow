using System;
using System.Collections.Generic;
using System.Reflection;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Configuration.Requests;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Models;
using Meadow.NullCore;
using Meadow.Requests;
using Meadow.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow
{
    public class MeadowEngine
    {
        private readonly MeadowConfiguration _configuration;
        private static ILogger _logger = NullLogger.Instance;


        private static IMeadowDataAccessCoreProvider _coreProvider = new CoreProvider<NullMeadowCore>();
        public Assembly MeadowRunnerAssembly { get; }


        public MeadowEngine(MeadowConfiguration configuration, Assembly meadowRunnerAssembly)
        {
            MeadowRunnerAssembly = meadowRunnerAssembly;
            _configuration = configuration;
        }

        public MeadowEngine(MeadowConfiguration configuration) :
            this(configuration, Assembly.GetCallingAssembly())
        {
        }


        public static void UseDataAccess(IMeadowDataAccessCoreProvider provider)
        {
            if (provider == null)
            {
                _coreProvider = new CoreProvider<NullMeadowCore>();
            }
            else
            {
                _coreProvider = provider;
            }
        }

        public static void UseLogger(ILogger logger)
        {
            if (logger == null)
            {
                _logger = NullLogger.Instance;
            }
            else
            {
                _logger = logger;
            }
        }

        private IMeadowDataAccessCore CreateInitializedCore(MeadowConfiguration configuration)
        {
            return _coreProvider.CreateDataAccessCore().Initialize(configuration, _logger);
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
            return CreateInitializedCore(_configuration).PerformRequest(request, _configuration);
        }

        private ConfigurationRequestResult PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request)
            where TOut : class, new()
        {
            try
            {
                var config = request.PreConfigure(_configuration);
                // Run Configuration Request As a Script Request
                var meadowRequest = CreateInitializedCore(_configuration).PerformRequest(request, config);

                return new ConfigurationRequestResult
                {
                    Success = !meadowRequest.Failed,
                    Exception = meadowRequest.FailureException
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
            CreateInitializedCore(_configuration)
                .CreateDatabase(_configuration);

            PerformPostDatabaseCreationTasks();
        }

        private void PerformPostDatabaseCreationTasks()
        {
            var core = CreateInitializedCore(_configuration);

            core.CreateTable<MeadowDatabaseHistory>(_configuration);

            core.CreateInsertProcedure<MeadowDatabaseHistory>(_configuration);

            core.CreateLastInsertedProcedure<MeadowDatabaseHistory>(_configuration);
        }

        public void DropDatabase()
        {
            CreateInitializedCore(_configuration)
                .DropDatabase(_configuration);
        }

        public bool DatabaseExists()
        {
            return CreateInitializedCore(_configuration)
                .DatabaseExists(_configuration);
        }

        public void CreateIfNotExist()
        {
            var created = CreateInitializedCore(_configuration)
                .CreateDatabaseIfNotExists(_configuration);

            if (created)
            {
                PerformPostDatabaseCreationTasks();
            }
        }


        private TModel ReadLastInsertedRecord<TModel>() where TModel : class, new()
        {
            var response = PerformRequest(new ReadLastModel<TModel>());

            if (response.FromStorage != null && response.FromStorage.Count == 1)
            {
                return response.FromStorage[0];
            }

            return default;
        }

        /// <summary>
        /// Applies all available buildup scripts
        /// </summary>
        /// <returns>A list of log reports</returns>
        public void BuildUpDatabase()
        {
            CreateInitializedCore(_configuration);
            
            var lastExecResult = ReadLastInsertedRecord<MeadowDatabaseHistory>();

            int lastAppliedOrder = -1;

            if (lastExecResult != null)
            {
                _logger.LogInformation("Already built up, up to {LastExecResultScriptName}:{LastExecResultScriptOrder}",
                    lastExecResult.ScriptName, lastExecResult.ScriptOrder);

                lastAppliedOrder = lastExecResult.ScriptOrder;
            }

            var manager = new BuildupScriptManager(_configuration.BuildupScriptDirectory, MeadowRunnerAssembly);

            if (manager.ScriptsCount == 0)
            {
                _logger.LogError(
                    "No valid build-up scripts where found at given directory {ConfigurationBuildupScriptDirectory}",
                    _configuration.BuildupScriptDirectory);
                return;
            }

            var anyApplied = false;

            for (int i = 0; i < manager.ScriptsCount; i++)
            {
                var info = manager[i];

                if (info.OrderIndex > lastAppliedOrder)
                {

                    _logger.LogInformation("Applying {InfoOrder}:{InfoName}",
                        info.Order,info.Name);

                    var result = PerformScript(info);

                    if (result.Success)
                    {

                        _logger.LogInformation("{InfoOrder}:{InfoName} has been applied successfully.",
                            info.Order,info.Name);

                        anyApplied = true;

                        PerformRequest(new MarkExecutionInHistoryRequest(info));
                    }
                    else
                    {

                        _logger.LogError(result.Exception, 
                            "Error Occured While Applying {InfoOrder}:{InfoName}\n {Exception}",
                        info.Order,info.Name,result.Exception);
                        
                        _logger.LogError("*** Buildup process FAILED at {InfoOrder}.***", info.Order);
                        
                        return;
                    }
                }
            }

            if (anyApplied)
            {
                _logger.LogInformation(@"*** Buildup process SUCCEEDED ***");
            }
            else
            {
                _logger.LogInformation(@"*** Everything Already Up-to-date ***");
            }
        }


        public List<string> EnumerateProcedures()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateProcedures(_configuration);
        }

        public List<string> EnumerateTables()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateTables(_configuration);
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