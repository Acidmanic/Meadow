using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class IdShell
    {
        public long Id { get; set; }
    }

    public class GetPersonByIdEager : MeadowRequest<IdShell, Person>
    {
        public GetPersonByIdEager(long id) : base(true)
        {
            this.ToStorage = new IdShell {Id = id};
        }

        protected override bool EagerReadWrite()
        {
            return true;
        }
    }
}