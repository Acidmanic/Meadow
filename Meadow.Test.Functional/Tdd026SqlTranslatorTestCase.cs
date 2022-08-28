using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Sql;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Tools.Assistant.Extensions;

namespace Meadow.Test.Functional
{
    public class Tdd026SqlTranslatorTestCase : MeadowFunctionalTest
    {
        public override void Main()
        {
            // var storageData = new List<Record>().LoadCached<List<Record>>( new DirectoryInfo(".").FullName,"unable-to-translate.json");
            //
            // storageData[0].Add("StrategyParameters.StrategyParameterDescriptor.QuantifierId",2L);
            //
            // var standardData = new SqlStandardDataTranslator2().TranslateFromStorage(storageData, typeof(SupplementDal));
            //
            // List<SupplementDal> results = new List<SupplementDal>();
            //
            // foreach (var record in standardData)
            // {
            //     var evaluator = new ObjectEvaluator(typeof(SupplementDal));
            //
            //     evaluator.LoadStandardData(record);
            //
            //     var recordObject = evaluator.As<SupplementDal>();
            //
            //     if (recordObject != null)
            //     {
            //         results.Add(recordObject);
            //     }
            // }
            
            
            var translator = new RelationalFieldAddressIdentifierTranslator(){Separator = "_"};
            
            var map = translator.MapAddressesByIdentifier<SupplementDal>();
            
            foreach (var item in map)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }
        }
    }
}