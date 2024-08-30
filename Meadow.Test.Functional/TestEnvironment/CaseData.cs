using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Requests.GenericEventStreamRequests.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public class CaseData
{
    public IReadOnlyDictionary<Type, List<object>> SeedsByType => _seedsByType;
    public IReadOnlyDictionary<object, List<StreamEvent>> EventsByStreamId => _eventsByStreamId;

    private readonly Dictionary<Type, List<object>> _seedsByType;
    private readonly Dictionary<object, List<StreamEvent>> _eventsByStreamId;


    private CaseData(Dictionary<Type, List<object>> seedsByType, Dictionary<object, List<StreamEvent>> eventsByStreamId)
    {
        _seedsByType = seedsByType;
        _eventsByStreamId = eventsByStreamId;
    }

    public List<T> Get<T>()
    {
        if (_seedsByType.ContainsKey(typeof(T)))
        {
            return new List<T>(_seedsByType[typeof(T)].Select(o => (T)o));
        }

        return new List<T>();
    }

    public List<T> Get<T>(Func<T, bool> predicate) => Get<T>().Where(predicate).ToList();


    public List<T> Get<T>(Comparison<T> comparison) => Sort(Get<T>(), comparison);


    private List<TModel> Sort<TModel>(IEnumerable<TModel> items, Comparison<TModel> compare)
    {
        var list = new List<TModel>(items);

        list.Sort(compare);

        return list;
    }

    public List<StreamEvent> Events<TStreamId>(TStreamId streamId)
    {
        if (_eventsByStreamId.ContainsKey(streamId!))
        {
            return _eventsByStreamId[streamId!];
        }

        return new List<StreamEvent>();
    }
    
    public List<StreamEvent> Events()
    {
        var events = new List<StreamEvent>();

        foreach (var eSet in _eventsByStreamId)
        {
            events.AddRange(eSet.Value);
        }

        return events;
    }

    public static CaseData Create(ICaseDataProvider provider)
    {
        provider.Initialize();

        var dataSets = provider.SeedSet;

        return Create(dataSets);
    }

    public static CaseData Create(List<List<object>> dataSets)
    {
        var seedsByType = new Dictionary<Type, List<object>>();
        var eventsByStreamId = new Dictionary<object, List<StreamEvent>>();
        var streamEventType = typeof(StreamEvent);

        foreach (var dataSet in dataSets)
        {
            if (dataSet.Count > 0)
            {
                var type = dataSet.First().GetType();

                if (type == streamEventType)
                {
                    foreach (var eObject in dataSet)
                    {
                        if (eObject is StreamEvent streamEvent)
                        {
                            if (!eventsByStreamId.ContainsKey(streamEvent.StreamId))
                            {
                                eventsByStreamId.Add(streamEvent.StreamId, new List<StreamEvent>());
                            }

                            eventsByStreamId[streamEvent.StreamId].Add(streamEvent);
                        }
                    }
                }
                else
                {
                    if (!seedsByType.ContainsKey(type))
                    {
                        seedsByType.Add(type, new List<object>());
                    }

                    foreach (var o in dataSet)
                    {
                        seedsByType[type].Add(o);
                    }
                }
            }
        }

        return new CaseData(seedsByType, eventsByStreamId);
    }
}