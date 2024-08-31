using System;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Extensions;
using Meadow.Attributes;
using Meadow.Extensions;

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
        
        var compression = eventType
            .GetHierarchicalCustomAttribute<EventStreamSerializationCompressionAttribute>();

        if (compression)
        {
            info.Compression = compression.Primary.Compression;
            info.CompressionLevel = compression.Primary.CompressionLevel;
        }
        
        var encoding = eventType
            .GetHierarchicalCustomAttribute<EventStreamSerializationEncodingAttribute>();

        if (encoding)
        {
            info.Encoding = encoding.Primary.Encoding;
        }

        return info;
    }
}