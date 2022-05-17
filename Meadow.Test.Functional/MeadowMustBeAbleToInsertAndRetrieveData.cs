using System;
using Meadow.Configuration;

namespace Meadow.Test.Functional
{
    public class MeadowMustBeAbleToInsertAndRetrieveData : IFunctionalTest
    {
        private static string connectionString =
            "Server=localhost;User Id=sa; Password=never54aga.1n;Database=MeadowDatabase; MultipleActiveResultSets=true";

        private class Tag
        {
            public long PropertyId { get; set; }

            public long ProductClassId { get; set; }
        }

        private class ReadAllTagsRequest : MeadowRequest<Tag, Tag>
        {
            public ReadAllTagsRequest() : base(true)
            {
                this.RequestText = "spReadAllTags";
            }
        }

        private class InsertNewTagRequest : MeadowRequest<Tag, Tag>
        {
            public InsertNewTagRequest(Tag newTag) : base(true)
            {
                this.RequestText = "spInsertTag";
                this.ToStorage = newTag;
            }
        }

        public void Main()
        {
            var engine = new MeadowEngine(new MeadowConfiguration {ConnectionString = connectionString});

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
                Console.WriteLine("Tag: " + tag.PropertyId + "," + tag.ProductClassId);
            }

            Console.WriteLine("----------------------------");

            Console.WriteLine(
                $@"Inserted value: ProductClass: {inserted.ProductClassId}, Property: {inserted.PropertyId}");
        }
    }
}