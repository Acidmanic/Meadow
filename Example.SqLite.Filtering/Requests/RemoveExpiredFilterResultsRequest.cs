using Example.SqLite.Filtering.Requests.Models;
using Meadow;
using Meadow.Requests;

namespace Example.SqLite.Filtering.Requests
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