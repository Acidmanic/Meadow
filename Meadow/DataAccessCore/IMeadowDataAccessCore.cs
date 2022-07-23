using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;

namespace Meadow.DataAccessCore
{
    public interface IMeadowDataAccessCore
    {
        MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new();


        void CreateDatabase(MeadowConfiguration configuration);

        void CreateDatabaseIfNotExists(MeadowConfiguration configuration);

        void DropDatabase(MeadowConfiguration configuration);

        bool DatabaseExists(MeadowConfiguration configuration);

        List<string> EnumerateProcedures(MeadowConfiguration configuration);

        List<string> EnumerateTables(MeadowConfiguration configuration);

        void CreateTable<TModel>(MeadowConfiguration configuration);

        void CreateInsertProcedure<TModel>(MeadowConfiguration configuration);

        void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration);

    }
}