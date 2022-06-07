using System;
using ConsoleAppFramework;

namespace Meadow.Tools.Assistant
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = ConsoleApp.Create(args);
            
            app.AddAllCommandType();
            
            app.Run();

            
        }
    }
}
