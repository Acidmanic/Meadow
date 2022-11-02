using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;
using Meadow.BuildupScripts;
using Meadow.Requests;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Nuget;
using Meadow.Tools.Assistant.Nuget.PackageSources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd032ListNugetReferences : MeadowFunctionalTest
    {
        public override void Main()
        {
            var logger = new ConsoleLogger().Enable(LogLevel.Debug);

            var nuget = new Nuget("nugetCache", logger);

            var directory = "nugetCache";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var projects = DotnetProjectInfo.FindProjects("../../../../");

            var mergedProject = new MergedProject(projects);

            var packages = mergedProject.PackageReferences;

            Console.WriteLine("--------------------------------------------");

            var ndl = new NuGetDownloader
            {
                Logger = logger
            }.FuckSsl();


            foreach (var packageReference in packages)
            {
                var pid = new PackageId
                {
                    Id = packageReference.PackageName,
                    Version = packageReference.PackageVersion
                };
                
                Result<byte[]> package;

                try
                {
                    package = ndl.ProvidePackage(pid);
                }
                catch (Exception e)
                {
                    logger.LogError(e,"Error downloading: {Exception}",e);

                    package = new Result<byte[]>().FailAndDefaultValue();
                }

                if (package)
                {
                    var path = Path.Combine(directory, pid.AsFileName());

                    JustWrite(path, package.Value);

                    Console.WriteLine("Downloaded: " +
                                      packageReference.PackageName + ": " +
                                      packageReference.PackageVersion);
                    
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine("Unable to Download from Nuget");
                }
            }

            Console.WriteLine("--------------------------------------------");
        }

        private void JustWrite(string path, byte[] data)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllBytes(path, data);
        }
    }
}