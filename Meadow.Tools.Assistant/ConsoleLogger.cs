using System;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant
{
    
    public class ConsoleLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = logLevel.ToString();

            message += ": " + formatter(state, exception);
                
            Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}