using Meadow.Scaffolding.Attributes;

namespace Example.SqLite.Models
{
    [EventStreamPreferences(typeof(long),typeof(long),128,256)]
    public interface IPersonEvent
    {
        
    }
}