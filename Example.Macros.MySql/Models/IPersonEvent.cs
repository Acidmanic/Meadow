using Meadow.Scaffolding.Attributes;

namespace Example.Macros.MySql.Models
{
    [EventStreamPreferences(typeof(long),typeof(long),256,1024)]
    public interface IPersonEvent
    {
        
    }
}