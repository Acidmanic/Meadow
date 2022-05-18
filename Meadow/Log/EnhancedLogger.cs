using System;

namespace Meadow.Log
{
    internal class EnhancedLogger:ILogger
    {

        private readonly ILogger _logger;

        public EnhancedLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string text)
        {
            _logger.Log(text);
        }
        
        public void LogException(Exception ex, string failedTitle)
        {
            _logger.Log(failedTitle + $@" has FAILED, due to {ex.GetType().Name}:");

            var lines = ex.Message.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                _logger.Log("\t\t" + line);
            }
        }
    }
}