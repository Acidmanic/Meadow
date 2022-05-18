using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Configuration.ConfigurationRequests;
using Meadow.Log;
using Meadow.Reflection;
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
        }

        /// <summary>
        /// Applies all available buildup scripts
        /// </summary>
        /// <returns>A list of log reports</returns>
        public void BuildUpDatabase()
        {
            var manager = new BuildupScriptManager(_configuration.BuildupScriptDirectory);

            if (manager.ScriptsCount == 0)
            {
                _logger.Log(
                    $@"No valid build-up scripts where found at given directory {_configuration.BuildupScriptDirectory}");
                return;
            }

            for (int i = 0; i < manager.ScriptsCount; i++)
            {
                var info = manager[i];

                var result = PerformScript(info);

                if (result.Success)
                {
                    _logger.Log($@"{info.Order}, {info.Name} has been applied successfully.");
                }
                else
                {
                    _logger.LogException(result.Exception, $@"Applying {info.Order}, {info.Name}");

                    _logger.Log($@"*** Buildup process FAILED at {info.Order}.***");

                    return;
                }

                _logger.Log($@"Applying {info.OrderIndex}, {info.Name}");
            }

            _logger.Log($@"*** Buildup process SUCCEEDED ***");

            return;
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