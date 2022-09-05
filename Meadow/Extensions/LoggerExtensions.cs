using Microsoft.Extensions.Logging;

namespace Meadow.Extensions
{
    public static class LoggerExtensions
    {
        public static ILogger UserForMeadow(this ILogger logger)
        {

            MeadowEngine.UseLogger(logger);

            return logger;
        }
    }
}