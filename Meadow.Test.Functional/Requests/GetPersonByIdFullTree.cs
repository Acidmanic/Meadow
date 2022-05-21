using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class IdShell
    {
        public long Id { get; set; }
    }

    public class GetPersonByIdFullTree : MeadowRequest<IdShell, Person>
    {
        public GetPersonByIdFullTree(long id) : base(true)
        {
            this.ToStorage = new IdShell {Id = id};
        }

        protected override bool FullTreeReadWrite()
        {
            return true;
        }
    }
}