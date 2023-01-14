using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Extensions;
using Meadow.MySql.Comments;
using Meadow.Sql;
using Meadow.Test.Functional.Models.Bug2;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.Utility;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd36UnExpectedRepeatation : MeadowFunctionalTest
    {
        private class UseCaseCsv : CsvData
        {
            public UseCaseCsv()
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


        public override void Main()
        {
            var path = Path.Combine("TestCaseData", "douplicated-data.csv");

            var csv = new UseCaseCsv();

            csv.Load(path);

            var relationalData = csv.Data;

            var mapper = new RelationalRelationalIdentifierToStandardFieldMapper
            {
                Separator = '_',
                DataOwnerNameProvider = new PluralDataOwnerNameProvider()
            };

            var standardUnIndexed = relationalData.RelationalToStandard<Project>(mapper, true);

            var acc = new StandardIndexAccumulator<Project>();

            acc.PassAll(standardUnIndexed);

            var indexedStandard = acc.Records;

            var results = new List<Project>();

            foreach (var record in indexedStandard)
            {
                var evaluator = new ObjectEvaluator(typeof(Project));

                evaluator.LoadStandardData(record);

                var recordObject = evaluator.As<Project>();

                if (recordObject != null)
                {
                    results.Add(recordObject);
                }
            }

            Console.WriteLine("----------------------------------------");

            foreach (var record in indexedStandard)
            {
                foreach (var datapoint in record)
                {
                    Console.WriteLine(datapoint);
                }
            }

            var p = new Project
            {
                Description = results[0].Description,
                Id = results[0].Id,
                Iterations = results[0].Iterations,
                Name = results[0].Name,
                Goals = new List<Goal>
                {
                    results[0].Goals[0],
                    results[0].Goals[1]
                }
            };

            var ev = new ObjectEvaluator(p);

            var expected = ev.ToStandardFlatData(o => o.IncludeNulls().FullTree());

            Console.WriteLine("----------------------------------------");


            foreach (var datapoint in expected)
            {
                Console.WriteLine(datapoint);
            }
        }
    }
}