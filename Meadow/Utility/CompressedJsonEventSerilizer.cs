using System;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Extensions;
using Meadow.Contracts;
using Newtonsoft.Json;

namespace Meadow.Utility;

public class CompressedJsonEventSerialization : IEventSerialization
{
    public async Task<string> Serialize(object eventObject,
        Encoding encoding,
        Compressions compression,
        CompressionLevel compressionLevel)
    {
        var serialized = JsonConvert.SerializeObject(eventObject);

        if (compression == Compressions.Brotli || compression == Compressions.GZip)
        {
            serialized = await serialized.CompressB64Async(compression, compressionLevel, encoding);
        }

        return serialized;
    }

    public async Task<object?> Deserialize(string serialized, Type type, Encoding encoding, Compressions compression)
    {
        if (compression == Compressions.Brotli || compression == Compressions.GZip)
        {
            serialized = await serialized.DecompressB64Async(compression, encoding);
        }

        return JsonConvert.DeserializeObject(serialized, type);
    }
}