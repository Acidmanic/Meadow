using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Sql;
using Meadow.Test.Functional.Models.Bug2;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.Utility;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd36UnExpectedRepeatation : MeadowFunctionalTest
    {
        private class ProjectsUseCaseCsv : CsvData
        {
            public ProjectsUseCaseCsv()
            {
                //Projects_Name
                AddColumnType<string>();
                //Projects_Id
                AddColumnType<long>();
                //Projects_Description
                AddColumnType<string>();
                //Goals_Id
                AddColumnType<long>();
                //Goals_ProjectId
                AddColumnType<long>();
                //Goals_Title
                AddColumnType<string>();
                //Goals_Description
                AddColumnType<string>();
                //Steps_Id
                AddColumnType<long>();
                //Steps_GoalId
                AddColumnType<long>();
                //Steps_ProjectId
                AddColumnType<long>();
                //Steps_Title
                AddColumnType<string>();
                //Steps_Description
                AddColumnType<string>();
                //Tasks_Id
                AddColumnType<long>();
                //Tasks_IterationId
                AddColumnType<long>();
                //Tasks_GoalId
                AddColumnType<long>();
                //Tasks_StepId
                AddColumnType<long>();
                //Tasks_ProjectId
                AddColumnType<long>();
                //Tasks_Title
                AddColumnType<string>();
                //Tasks_Description
                AddColumnType<string>();
                //Iterations_ProjectId
                AddColumnType<long>();
                //Iterations_Id
                AddColumnType<long>();
                //Iterations_Description
                AddColumnType<string>();
                //Iterations_Name
                AddColumnType<string>();
            }
        }

        private class IterationsUseCaseCsv : CsvData
        {
            public IterationsUseCaseCsv()
            {
                //Iterations_Id
                AddColumnType<long>();
                //Iterations_ProjectId
                AddColumnType<long>();
                //Iterations_Name
                AddColumnType<string>();
                //Iterations_Description
                AddColumnType<string>();
                //Tasks_Description
                AddColumnType<string>();
                //Tasks_ProjectId
                AddColumnType<long>();
                //Tasks_Id
                AddColumnType<long>();
                //Tasks_IterationId
                AddColumnType<long>();
                //StepId
                AddColumnType<long>();
                //Tasks_GoalId
                AddColumnType<long>();
                //Tasks_Title
                AddColumnType<string>();
            }
        }
        
        public override void Main()
        {

            var items = new List<string> { " - " ,"1-1", "1-3" , "2-2" , "2-4" , "1-5" , "2-6" , "1-7" , "2-8"};
            
            
            StandardIndexAccumulator<Iteration>.MoveInto(items,5,2);
            
            
            var path = Path.Combine("TestCaseData", "itr-should-have-tasks.csv");

            var csv = new IterationsUseCaseCsv();

            csv.Load(path);

            var relationalData = csv.Data;

            var mapper = new ConditionalRelationalToStandardMapper
            {
                Separator = '_',
                DataOwnerNameProvider = new PluralDataOwnerNameProvider()
            };

            var standardUnIndexed = relationalData.RelationalToStandard<Iteration>(mapper, true);

            var acc = new StandardIndexAccumulator<Iteration>(new ConsoleLogger().Shorten().EnableAll());

            acc.LogVerbose = true;
            
            acc.PassAll(standardUnIndexed);

            var indexedStandard = acc.Records;

            var results = new List<Iteration>();

            foreach (var record in indexedStandard)
            {
                var evaluator = new ObjectEvaluator(typeof(Iteration));

                evaluator.LoadStandardData(record);

                var recordObject = evaluator.As<Iteration>();

                if (recordObject != null)
                {
                    results.Add(recordObject);
                }
            }

            Console.WriteLine("----------------------------------------");

            
        }
    }
}