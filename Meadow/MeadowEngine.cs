using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Configuration.Requests;
using Meadow.Contracts;
using Meadow.Models;
using Meadow.NullCore;
using Meadow.Requests;
using Meadow.Requests.Configuration;
using Meadow.Requests.Configuration.Abstractions;
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


        private void SetupQueryTranslator<TOut>(MeadowRequest<TOut> request)
        {
            var translator = ISqlFilteringTranslator.NullSqlExpressionTranslator.Instance;

            try
            {
                var core = _coreProvider.CreateDataAccessCore();

                var t = core.ProvideSqlLanguageTranslator();

                if (t != null)
                {
                    translator = t;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting query translator.");
            }

            translator.Logger = _logger;

            translator.Configuration = _configuration;

            request.SetFilterQueryTranslator(translator);
        }

        public MeadowRequest<TOut> PerformRequest<TOut>(MeadowRequest<TOut> request)
        {
            request.SetConfigurations(_configuration);

            SetupQueryTranslator(request);

            if (request is ConfigurationRequest<TOut> configRequest)
            {
                var result = PerformConfigurationRequest(configRequest);

                configRequest.Result = result;

                return request;
            }

            // Run UserRequest as Procedure Request
            return CreateInitializedCore(_configuration).PerformRequest(request, _configuration);
        }

        public async Task<MeadowRequest<TOut>> PerformRequestAsync<TOut>(MeadowRequest<TOut> request)
            where TOut : class, new()
        {
            request.SetConfigurations(_configuration);

            SetupQueryTranslator(request);

            if (request is ConfigurationRequest<TOut> configRequest)
            {
                var result = await PerformConfigurationRequestAsync(configRequest);

                configRequest.Result = result;

                return request;
            }

            return await CreateInitializedCore(_configuration).PerformRequestAsync(request, _configuration);
        }

        private ConfigurationRequestResult PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request)
            where TOut : class
        {
            try
            {
                var config = request.PreConfigure();
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

        private async Task<ConfigurationRequestResult> PerformConfigurationRequestAsync<TOut>(
            ConfigurationRequest<TOut> request) where TOut : class, new()
        {
            try
            {
                var config = request.PreConfigure();
                // Run Configuration Request As a Script Request
                var meadowRequest = await CreateInitializedCore(_configuration).PerformRequestAsync(request, config);

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

            core.CreateReadAllProcedure<MeadowDatabaseHistory>(_configuration);
        }


        private async Task PerformPostDatabaseCreationTasksAsync()
        {
            var core = CreateInitializedCore(_configuration);

            await core.CreateTableAsync<MeadowDatabaseHistory>(_configuration);

            await core.CreateInsertProcedureAsync<MeadowDatabaseHistory>(_configuration);

            await core.CreateLastInsertedProcedureAsync<MeadowDatabaseHistory>(_configuration);

            await core.CreateReadAllProcedureAsync<MeadowDatabaseHistory>(_configuration);
        }

        public void DropDatabase()
        {
            CreateInitializedCore(_configuration)
                .DropDatabase(_configuration);
        }

        public Task DropDatabaseAsync()
        {
            return CreateInitializedCore(_configuration)
                .DropDatabaseAsync(_configuration);
        }

        public bool DatabaseExists()
        {
            return CreateInitializedCore(_configuration)
                .DatabaseExists(_configuration);
        }

        public Task<bool> DatabaseExistsAsync()
        {
            return CreateInitializedCore(_configuration)
                .DatabaseExistsAsync(_configuration);
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

        public async Task CreateIfNotExistAsync()
        {
            var created = await CreateInitializedCore(_configuration)
                .CreateDatabaseIfNotExistsAsync(_configuration);

            if (created)
            {
                await PerformPostDatabaseCreationTasksAsync();
            }
        }


        private async Task<TModel?> ReadLastInsertedRecordAsync<TModel>() where TModel : class, new()
        {
            var response = await PerformRequestAsync(new ReadLastModel<TModel>());

            if (response.FromStorage.Count == 1)
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
            BuildUpDatabaseAsync().Wait();
        }

        /// <summary>
        /// Applies all available buildup scripts
        /// </summary>
        /// <returns>A list of log reports</returns>
        public async Task BuildUpDatabaseAsync()
        {
            CreateInitializedCore(_configuration);

            var lastExecResult = await ReadLastInsertedRecordAsync<MeadowDatabaseHistory>();

            int lastAppliedOrder = -1;

            if (lastExecResult != null)
            {
                _logger.LogInformation("Already built up, up to {LastExecResultScriptName}:{LastExecResultScriptOrder}",
                    lastExecResult.ScriptName, lastExecResult.ScriptOrder);

                lastAppliedOrder = lastExecResult.ScriptOrder;
            }

            var manager = CreateBuildupScriptManager();

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
                        info.Order, info.Name);

                    var result = await PerformScriptAsync(info);

                    if (result.Success)
                    {
                        _logger.LogInformation("{InfoOrder}:{InfoName} has been applied successfully.",
                            info.Order, info.Name);

                        anyApplied = true;

                        await PerformRequestAsync(new MarkExecutionInHistoryRequest(info));
                    }
                    else
                    {
                        _logger.LogError(result.Exception,
                            "Error Occured While Applying {InfoOrder}:{InfoName}\n {Exception}",
                            info.Order, info.Name, result.Exception);

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

        private BuildupScriptManager CreateBuildupScriptManager()
        {
            var assemblies = new List<Assembly>(_configuration.MacroContainingAssemblies);

            // Builtin assemblies
            assemblies.Add(this.GetType().Assembly);
            assemblies.Add(Assembly.GetEntryAssembly());

            return new BuildupScriptManager(_configuration.BuildupScriptDirectory,
                _configuration, assemblies.ToArray());
        }


        public List<string> EnumerateProcedures()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateProcedures(_configuration);
        }

        public Task<List<string>> EnumerateProceduresAsync()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateProceduresAsync(_configuration);
        }

        public List<string> EnumerateTables()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateTables(_configuration);
        }

        public Task<List<string>> EnumerateTablesAsync()
        {
            return CreateInitializedCore(_configuration)
                .EnumerateTablesAsync(_configuration);
        }


        private async Task<ConfigurationRequestResult> PerformScriptAsync(ScriptInfo scriptInfo)
        {
            var sqls = scriptInfo.SplitScriptIntoBatches();

            foreach (var sql in sqls)
            {
                var request = new SqlCommandRequest(sql);

                var result = await PerformConfigurationRequestAsync(request);

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