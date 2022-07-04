using Meadow.Configuration;
using Meadow.Requests;

namespace Meadow.DataAccessCore
{
    public interface IMeadowDataAccessCore
    {
        MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new();
    }
}