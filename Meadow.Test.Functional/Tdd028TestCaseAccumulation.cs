using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Extensions;
using Meadow.RelationalTranslation;
using Meadow.Sql;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Tools.Assistant.Extensions;

namespace Meadow.Test.Functional
{
    public class Tdd028TestCaseAccumulation : MeadowFunctionalTest
    {

        private class MostInner
        {
            [AutoValuedMember]
            [UniqueMember]
            public  long Id { get; set; }
        }

        private class InnerModel
        {
            [AutoValuedMember]
            [UniqueMember]
            public  long Id { get; set; }
            
            public MostInner MostInner { get; set; }
        }

        private class OuterModel
        {
            [AutoValuedMember]
            [UniqueMember]
            public long Id { get; set; }
            
            public List<InnerModel> Inners { get; set; } 
            
        }
        
        
        public override void Main()
        {
             

                // var trans = new RelationalFieldAddressIdentifierTranslator()
                // {
                //     Separator = '_',
                //     DataOwnerNameProvider = new PluralDataOwnerNameProvider()
                // };
                //
                // var adapt = new FieldAddressTranslatedStandardDataTranslator(trans);
                //
                // var record = new Record();
                //
                // record.Add("OuterModels_Id",1);
                // record.Add("InnerModels_Id",11);
                // record.Add("MostInners_Id",111);
                // record.Add("InnerModels_Id",12);
                // record.Add("MostInners_Id",121);
                //
                // var fromStorage = adapt.TranslateFromStorage(new List<Record>{record}, typeof(OuterModel),true);
                //
                // var evaluator = new ObjectEvaluator(typeof(OuterModel));
                //
                // evaluator.LoadStandardData(fromStorage[0]);
                //
                // var reconstructed = evaluator.RootObject;
                
                var obj = new OuterModel
                {
                    Id = 1,
                    Inners = new List<InnerModel>
                    {
                        new InnerModel
                        {
                            Id = 11,
                            MostInner = new MostInner
                            {
                                Id = 111
                            }
                        },
                        new InnerModel
                        {
                            Id = 12,
                            MostInner = new MostInner
                            {
                                Id = 121
                            }
                        }
                    }
                };
                
                var standardData = new ObjectEvaluator(obj).ToStandardFlatData();

                var indexLessData = new Record();
                
                standardData.ForEach(dp => indexLessData.Add(FieldKey.Parse(dp.Identifier).ClearIndexes().ToString(),dp.Value));
                
                var tr = new StandardIndexAccumulator<OuterModel>();
                
                indexLessData.ForEach(dp => tr.Pass(dp));

                var reconstructed = tr.DeliverShit();
        }
    }
}