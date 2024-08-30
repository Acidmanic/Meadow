using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests.GenericEventStreamRequests.Models;

namespace Meadow.Requests.GenericEventStreamRequests;

public abstract class EventStreamRequestBase<TEvent,TEventId,TStreamId,TIn>:MeadowRequest<TIn,ObjectEntry<TEventId,TStreamId>>
{

    public EventStreamRequestBase() : base(true)
    {
    }


    private NameConvention NameConvention => Configuration.GetNameConvention<TEvent>();

    protected abstract string PickName(NameConvention nameConvention);
    
    public override string RequestText
    {
        get => PickName(NameConvention);
        protected set { }
    }
}