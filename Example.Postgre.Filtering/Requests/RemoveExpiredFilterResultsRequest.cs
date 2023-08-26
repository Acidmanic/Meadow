using Example.Postgre.Filtering.Requests.Models;
using Meadow;
using Meadow.Requests;

namespace Example.Postgre.Filtering.Requests
{
    public sealed class RemoveExpiredFilterResultsRequest:MeadowRequest<ExpirationTimeStampShell,MeadowVoid>
    {
        public RemoveExpiredFilterResultsRequest(long expirationTimeStamp) : base(false)
        {
            ToStorage = new ExpirationTimeStampShell
            {
                ExpirationTimeStamp = expirationTimeStamp
            };
        }
    }
}