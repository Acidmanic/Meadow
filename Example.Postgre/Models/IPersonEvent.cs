using Meadow.Scaffolding.Attributes;

namespace Example.Postgre.Models
{
    [EventStreamPreferences(typeof(long),typeof(long),256,1024)]
    public interface IPersonEvent
    {
        
    }
}