using Meadow.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Example.SqlServer.Models
{
    [EventStreamPreferences(typeof(long),typeof(long),256,1024)]
    public interface IPersonEvent
    {
        
    }
}