using System;
using CoreCommandLine.DotnetDi;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Tools.Assistant
{
    class Program
    {
        static void Main(string[] args)
        {

            var logger = new ConsoleLogger().Shorten();

            var app = new DotnetCommandlineApplicationBuilder<MatApplication>()
                .Describe("Meadow Assistant Tool", "Helper Tool For Meadow Framework.")
                .UseLogger(logger).UseStartup<Startup>()
                .Build();
            
            
            app.ExecuteInteractive();
            
        }
    }
}
