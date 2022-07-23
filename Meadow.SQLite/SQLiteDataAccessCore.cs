using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Requests;
using Meadow.Scaffolding.Sqlable;
using Meadow.SQLite.CarrierInterceptors;
using Meadow.SQLite.Requests;
using Meadow.SQLite.SqlScriptsGenerators;
using Meadow.Utility;

namespace Meadow.SQLite
{
    public class SqLiteDataAccessCore : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } =
            new SqLiteStorageAdapter();

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } =
            new SQLiteStorageCommunication();

        public SqLiteDataAccessCore()
        {
            AddCarrierInterceptor(new SQLiteCommandInterceptor());
        }
        
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            TryDbFile(configuration,file =>
            {
                if (File.Exists(file))
                {
                    throw new Exception("The Database already exists");
                }
                PerformRequest(new CreateDatabaseRequest(), configuration);
            });
        }

        public override void CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            TryDbFile(configuration,file =>
            {
                if (!File.Exists(file))
                {
                    PerformRequest(new CreateDatabaseRequest(), configuration);
                    
                }
            });
        }


        // protected override IDbCommand ProvideCarrier<TIn, TOut>(MeadowRequest<TIn, TOut> request, MeadowConfiguration configuration)
        // {
        //     var storage = request.ToStorage;
        //
        //     if (request.Execution == RequestExecution.RequestTextIsNameOfRoutine && storage == null)
        //     {
        //         var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);
        //         
        //         
        //         var procedure = SqLiteInMemoryProcedures.Instance.GetProcedure(request.RequestText);
        //
        //         carrier.CommandText = procedure.Code;
        //
        //         return carrier;
        //     }
        //     return base.ProvideCarrier(request, configuration);
        // }

        private void TryDbFile(MeadowConfiguration configuration, Action<string> code)
        {
            var conInfo = new ConnectionStringParser().Parse(configuration.ConnectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"];

                try
                {
                    code(filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            TryDbFile(configuration, file =>
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    
                }
            });
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            var exists = false;

            TryDbFile(configuration, file =>
            {
                exists = File.Exists(file);
            });

            return exists;
        }

        public override List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return SqLiteInMemoryProcedures.Instance.ListProcedures();
        }

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            var response = PerformRequest(new EnumerateTablesRequest(), configuration);
            
            var result  = new List<string>();

            if (response.FromStorage != null)
            {
                result.AddRange(response.FromStorage.Select(nr => nr.Name));
            }

            return result;
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new TableScriptGenerator(type).Generate().Text;
            
            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new InsertProcedureGenerator(type).Generate().Text;
            
            script = ClearGo(script);

            var procedure = SqLiteProcedure.Parse(script);
            
            SqLiteInMemoryProcedures.Instance.AddProcedure(procedure);
            
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new ReadSequenceProcedureGenerator(type, false, 1, false).Generate().Text;
            
            script = ClearGo(script);
            
            var procedure = SqLiteProcedure.Parse(script);
            
            SqLiteInMemoryProcedures.Instance.AddProcedure(procedure);
        }
        
        private string ClearGo(string script)
        {
            return script
                .Split(new string[] {"GO","--SPLIT","go","Go","gO"}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                ?.Trim();
        }
    }
}