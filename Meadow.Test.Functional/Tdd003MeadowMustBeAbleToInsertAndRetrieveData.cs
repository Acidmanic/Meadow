using System;
using Meadow.Configuration;
using Meadow.Requests;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd003MeadowMustBeAbleToInsertAndRetrieveData : MeadowFunctionalTest
    {

        public Tdd003MeadowMustBeAbleToInsertAndRetrieveData():base("MeadowDatabase")
        { }
        
        private class Tag
        {
            public long PropertyId { get; set; }

            public long ProductClassId { get; set; }
        }

        private class ReadAllTagsRequest : MeadowRequest<Tag>
        {

            public ReadAllTagsRequest():base()
            {
            }

            public override string RequestText => "spReadAllTags";
        }

        private class InsertNewTagRequest : MeadowRequest<Tag>
        {
            public InsertNewTagRequest(Tag newTag) : base(newTag)
            { }

            public override string RequestText => "spInsertTag";
        }

        public override void Main()
        {
            var engine = CreateEngine();

            var request = new ReadAllTagsRequest();

            engine.PerformRequest(request);

            var tags = request.FromStorage;

            int count = tags.Count;


            var nt = new Tag
            {
                PropertyId = count * 100,
                ProductClassId = count * 200
            };

            var insert = new InsertNewTagRequest(nt);

            var inserted = engine.PerformRequest(insert).FromStorage[0];

            engine.PerformRequest(request);

            tags = request.FromStorage;

            foreach (var tag in tags)
            {
                PrintObject(tag);
            }

            Console.WriteLine("----------------------------");

            Console.WriteLine(
                $@"Inserted value: ProductClass: {inserted.ProductClassId}, Property: {inserted.PropertyId}");
        }
    }
}