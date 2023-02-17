namespace Meadow.DataSource;

public class EventStreamRecord<TEventId,TStreamId>
{
    
    public virtual TEventId EventId { get; set; }
    
    public virtual TStreamId StreamId { get; set; }
    
    public virtual string TypeName { get; set; }
    
    public virtual string SerializedValue { get; set; }
}