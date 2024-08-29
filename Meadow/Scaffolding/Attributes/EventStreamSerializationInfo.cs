using System;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Extensions;
using Meadow.Attributes;

namespace Meadow.Scaffolding.Attributes;

public class EventStreamSerializationInfo
{
    public Compressions Compression { get; private set; }

    public CompressionLevel CompressionLevel { get; private set;}
    
    public Encoding Encoding { get; private set; } = Encoding.Default;

    private EventStreamSerializationInfo()
    {
        
    }

    public static EventStreamSerializationInfo FromType<T>()
    {
        return FromType(typeof(T));
    }

    public static EventStreamSerializationInfo FromType(Type eventType)
    {

        var info = new EventStreamSerializationInfo()
        {
            Compression = Compressions.None,
            Encoding = Encoding.Default,
            CompressionLevel = CompressionLevel.NoCompression
        };
        
        var compressionAttribute = eventType.GetCustomAttribute<EventStreamSerializationCompressionAttribute>();

        if (compressionAttribute is {} comp)
        {
            info.Compression = comp.Compression;
            info.CompressionLevel = comp.CompressionLevel;
        }
        
        var encodingAttribute = eventType.GetCustomAttribute<EventStreamSerializationEncodingAttribute>();

        if (encodingAttribute is { } enc)
        {
            info.Encoding = enc.Encoding;
        }

        return info;
    }
}