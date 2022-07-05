using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataAccessCore;
using Meadow.Log;
using Meadow.Requests;

namespace Meadow.NullCore
{
    public class NullMeadowCore:IMeadowDataAccessCore
    {

        private readonly ILogger _logger;

        public NullMeadowCore(ILogger logger)
        {
            _logger = logger;
        }

        public NullMeadowCore():this(new ConsoleLogger())
        {
            
        }


        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request, MeadowConfiguration configuration) where TOut : class, new()
        {
            _logger.Log("No DataAccessCore has been introduced to meadow engine.");

            if (request.ReturnsValue)
            {
                request.FromStorage = new List<TOut>();
            }

            return request;
        }
    }
}