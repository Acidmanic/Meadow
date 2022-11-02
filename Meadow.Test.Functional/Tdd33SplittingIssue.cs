using System.IO;
using Meadow.BuildupScripts;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd33SplittingIssue:MeadowFunctionalTest
    {
        public override void Main()
        {

            
            
            var filepath = "Scripts/testcase-split.sql";

            var content = File.ReadAllText(filepath);

            var splitted = ScriptInfoExtensions.SplitScriptIntoBatches(content);


        }
    }
}