using System;
using System.Collections.Generic;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Configuration.ConfigurationRequests;
using Meadow.Configuration.Requests;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Log;
using Meadow.Models;
using Meadow.NullCore;
using Meadow.Requests;
using Meadow.Utility;

namespace Meadow
{
    public class MeadowEngine
    {
        private readonly MeadowConfiguration _configuration;
        private readonly EnhancedLogger _logger;


        private static IMeadowDataAccessCoreProvider _coreProvider = new CoreProvider<NullMeadowCore>();
        
        public MeadowEngine(MeadowConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = new EnhancedLogger(logger);
            
        }

        public MeadowEngine(MeadowConfiguration configuration) : this(configuration, new NullLogger())
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

        private IMeadowDataAccessCore CreateInitializedCore(MeadowConfiguration configuration)
        {
            return _coreProvider.CreateDataAccessCore().Initialize(configuration);
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

        
        private  TModel ReadLastInsertedRecord<TModel>() where TModel : class, new()
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
            var core = CreateInitializedCore(_configuration);


            var lastExecResult = ReadLastInsertedRecord<MeadowDatabaseHistory>();

            int lastAppliedOrder = -1;

            if (lastExecResult!=null)
            {
                _logger.Log($"Already built up, up to {lastExecResult.ScriptName}:{lastExecResult.ScriptOrder}");

                lastAppliedOrder = lastExecResult.ScriptOrder;
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