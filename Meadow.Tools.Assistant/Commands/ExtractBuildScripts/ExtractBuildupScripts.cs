using System.IO;
using Acidmanic.Utilities.Extensions;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Meadow.Models;
using Meadow.Tools.Assistant.Commands.Arguments;
using Meadow.Tools.Assistant.Commands.ProjectAssembly;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.ExtractBuildScripts
{
    [CommandName("extract-scripts", "es")]
    [Subcommands(typeof(TargetProjectPath),
        typeof(ScriptsDirectory),
        typeof(DatabaseType), typeof(ConfigurationArgument))]
    public class ExtractBuildupScripts : HubCommandBase
    {
        public override string Description => "Connects to the target project's database, reads the meadow history " +
                                              "(if any) and re creates the buildup scripts in scripts-directory. ";

        protected override void Execute(Context context, CommandArguments arguments)
        {
            var helper = new ProjectAssemblyHelper(Logger, Output);

            var meadowConfigurations = helper.GetMeadowConfiguration(context);

            if (meadowConfigurations)
            {
                var engine = new MeadowEngine(meadowConfigurations.Value);

                MeadowEngine.UseLogger(Logger);

                context.SetDatabaseType(engine);

                var request = new ReadAllHistoryRequest();

                var response = engine.PerformRequest(request);


                if (!response.Failed)
                {
                    var scriptDirectory = context.GetScriptsDirectoryPath();

                    response.FromStorage.ForEach(h =>
                    {
                        Logger.LogInformation(h.ScriptName);

                        var script = h.Script.DecompressAsync(Compressions.GZip).Result;

                        File.WriteAllText(Path.Combine(scriptDirectory, FileName(h)), script);
                    });
                }
                else
                {
                    Logger.LogError(response.FailureException, "Error extracting data: {Exception}",
                        response.FailureException);
                }
            }
            else
            {
                Logger.LogError("Unable to find meadow configuration provider. Extraction aborted.");
            }
        }


        private string Pad(int number, int digits)
        {
            var padded = number.ToString();

            while (padded.Length < digits)
            {
                padded = "0" + padded;
            }

            return padded;
        }

        private string FileName(MeadowDatabaseHistory historyItem)
        {
            return Pad(historyItem.ScriptOrder, 4) + "-" + historyItem.ScriptName + ".sql";
        }
    }
}