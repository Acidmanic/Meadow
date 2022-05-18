using System;

namespace Meadow.Log
{
    public class ConsoleLogger:ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}