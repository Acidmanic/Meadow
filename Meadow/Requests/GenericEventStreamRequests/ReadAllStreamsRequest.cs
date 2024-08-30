using System;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests.GenericEventStreamRequests.Models;

namespace Meadow.Requests.GenericEventStreamRequests;

public class ReadAllStreamsRequest<TEventId, TStreamId> :
    MeadowRequest<MeadowVoid,ObjectEntry<TEventId,TStreamId>>

{
    private readonly Type _eventType;
    
    public ReadAllStreamsRequest(Type eventType) : base(true)
    {
        this._eventType = eventType;
    }

    private string PickName(NameConvention nameConvention)
    {
        return nameConvention.ReadAllStreams;
    }
    
    private NameConvention NameConvention => Configuration.GetNameConvention(_eventType);
    
    public override string RequestText
    {
        get => PickName(NameConvention);
        protected set { }
    }
}

public sealed class ReadAllStreamsRequest<TEvent, TEventId, TStreamId> : ReadAllStreamsRequest<TEventId, TStreamId>
{
    public ReadAllStreamsRequest() : base(typeof(TEvent))
    {
        
    }
}
    