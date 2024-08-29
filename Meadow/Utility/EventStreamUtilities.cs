using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Utility;

public class EventStreamUtilities
{
    private static readonly object UpdateCacheLock = new();
    private static readonly Dictionary<Type, IEntryBuilder> BuildersByTypeCache;
    private static readonly Type ObjectEntryGenericType = typeof(ObjectEntry<,>);

    static EventStreamUtilities()
    {
        BuildersByTypeCache = new Dictionary<Type, IEntryBuilder>();
    }

    public static ObjectEntry<TEventId, TStreamId>? ToEntry<TEventId, TStreamId>(
        object? e,
        TStreamId streamId,
        MeadowConfiguration meadowConfiguration)
        => ToEntry(e, streamId, typeof(TEventId), typeof(TStreamId), meadowConfiguration) as
            ObjectEntry<TEventId, TStreamId>;

    public static object? ToEntry(object? e, object? streamId, Type eventIdType, Type streamIdType,
        MeadowConfiguration meadowConfiguration)
    {
        if (e is { } value && streamId is {} sId)
        {
            var eventType = value.GetType();

            lock (UpdateCacheLock)
            {
                if (!BuildersByTypeCache.ContainsKey(eventType))
                {
                    BuildersByTypeCache[eventType] = CreateBuilderForEventType
                    (eventType, e, sId, eventIdType, streamIdType,
                        meadowConfiguration.EventSerialization);
                }
            }

            var created = BuildersByTypeCache[eventType].Build(e, sId);

            if (created) return created.Value;
        }

        return null;
    }

    private static IEntryBuilder CreateBuilderForEventType(Type eventType,
        object e, object streamId, Type eventIdType, Type streamIdType,
        IEventSerialization serialization)
    {
        var preferencesInfo = EventStreamPreferencesInfo.FromType(eventType);

        if (preferencesInfo)
        {
            if (preferencesInfo.Value.EventIdType == eventIdType && preferencesInfo.Value.StreamIdType == streamIdType)
            {
                var foundConstructor = FindObjectEntryConstructor(eventIdType, streamIdType);

                if (foundConstructor is { } c)
                {
                    var serializationInfo = EventStreamSerializationInfo.FromType(eventType);

                    var actualBuilder = new Builder(
                        o => o.ReadIdOrDefault(eventType, eventIdType),
                        c, serialization, serializationInfo, eventType
                    );

                    var built = actualBuilder.Build(e, streamId);

                    if (built)
                    {
                        return actualBuilder;
                    }
                }
            }
        }

        return IEntryBuilder.Null;
    }


    private static ConstructorInfo? FindObjectEntryConstructor(Type eventIdType, Type streamIdType)
    {
        var objectEntryType = ObjectEntryGenericType.MakeGenericType(eventIdType, streamIdType);


        var constructor = objectEntryType.GetConstructor(new[]
        {
            eventIdType, streamIdType,
            typeof(string), typeof(string)
        });

        return constructor;
    }


    private interface IEntryBuilder
    {
        Result<object> Build(object e, object streamId);

        public static readonly IEntryBuilder Null = new NullEntryBuilder();

        private class NullEntryBuilder : IEntryBuilder
        {
            private static readonly Result<object> Failed = new Result<object>().FailAndDefaultValue();

            public Result<object> Build(object e, object streamId) => Failed;
        }
    }

    private class Builder : IEntryBuilder
    {
        private readonly Func<object, object?> _idReader;
        private readonly ConstructorInfo _constructorInfo;
        private readonly IEventSerialization _serialization;
        private readonly EventStreamSerializationInfo _serializationInfo;
        private readonly Type _eventType;

        public Builder(Func<object, object?> idReader,
            ConstructorInfo constructorInfo,
            IEventSerialization serialization, EventStreamSerializationInfo serializationInfo, Type eventType)
        {
            _idReader = idReader;
            _constructorInfo = constructorInfo;
            _serialization = serialization;
            _serializationInfo = serializationInfo;
            _eventType = eventType;
        }

        public Result<object> Build(object e, object streamId)
        {
            var eventId = _idReader(e);

            if (eventId is { } id)
            {
                var serializedEvent = _serialization.Serialize
                    (e, _serializationInfo.Encoding,
                        _serializationInfo.Compression,
                        _serializationInfo.CompressionLevel)
                    .Result;

                if (serializedEvent is { } serialized)
                {
                    try
                    {
                        var constructed = _constructorInfo.Invoke(new[]
                        {
                            id, streamId, _eventType.FullName, serialized
                        });

                        if (constructed is { } entry) return entry;
                    }
                    catch
                    {
                        /* ignore */
                    }
                }
            }

            return new Result<object>().FailAndDefaultValue();
        }
    }
}