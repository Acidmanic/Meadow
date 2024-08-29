using System;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Extensions;

namespace Meadow.Contracts;

public interface IEventSerialization
{
    Task<string> Serialize(object eventObject, 
        Encoding encoding,
        Compressions compression,
        CompressionLevel compressionLevel);
    
    Task<object?> Deserialize(string serialized, Type type, 
        Encoding encoding,
        Compressions compression);
    
    
}

public static class EventSerializationExtensions
{


    public static async Task<T?> Deserialize<T>(this IEventSerialization serialization,
        string serialized,
        Encoding encoding,
        Compressions compression)
    {
        var value =  await  serialization.Deserialize(serialized, typeof(T),encoding, compression);

        if (value is T tValue) return tValue;

        return default;
    }
}