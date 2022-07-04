using System;
using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow
{
    internal class MeadowDataAccessCore:MeadowDataAccessCoreBase<IDbCommand,IDataReader>
    {
        // private readonly IDataOwnerNameProvider _dataOwnerNameProvider;
        //
        // public MeadowDataAccessCore(IDataOwnerNameProvider dataOwnerNameProvider)
        // {
        //     _dataOwnerNameProvider = dataOwnerNameProvider;
        // }
        //
        //
        // protected IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } =
        //     new SqlDataStorageAdapter();
        //
        // protected IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } = new SqlCommunication();
        //
        // public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
        //     MeadowRequest<TIn, TOut> request,
        //     MeadowConfiguration configuration)
        //     where TOut : class, new()
        // {
        //     request.InitializeBeforeExecution(_dataOwnerNameProvider);
        //     
        //     
        //     var carrier = ProvideCarrier(request, configuration);
        //
        //
        //     void OnDataAvailable(IDataReader reader)
        //     {
        //         request.FromStorage = DataStorageAdapter.ReadFromStorage<TOut>(reader, request.FromStorageMarks);
        //     }
        //
        //     StorageCommunication.Communicate(carrier,OnDataAvailable, configuration,request.ReturnsValue);
        //
        //     return request;
        // }
        //
        // private IDbCommand ProvideCarrier<TIn, TOut>(
        //     MeadowRequest<TIn, TOut> request,
        //     MeadowConfiguration configuration)
        //     where TOut : class, new()
        // {
        //   
        //     var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);
        //     
        //     var storage = request.ToStorage;
        //
        //     if (storage is null)
        //     {
        //         return carrier;
        //     }
        //
        //     var evaluator = new ObjectEvaluator(storage);
        //
        //     DataStorageAdapter.WriteToStorage(carrier, request.ToStorageMarks, evaluator);
        //
        //     return carrier;
        // }
        public MeadowDataAccessCore(IDataOwnerNameProvider dataOwnerNameProvider) : base(dataOwnerNameProvider)
        {
        }

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } = new SqlDataStorageAdapter();
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } = new SqlCommunication();
    }
}