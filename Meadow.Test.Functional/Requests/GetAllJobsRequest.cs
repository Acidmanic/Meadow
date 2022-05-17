using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class GetAllJobsRequest:MeadowRequest<MeadowVoid,Job>
    {
        public GetAllJobsRequest() : base(true)
        {
        }
        
        
    }
}