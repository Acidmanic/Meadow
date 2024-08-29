using System;
using System.IO.Compression;
using Acidmanic.Utilities.Extensions;

namespace Meadow.Attributes
{
    public class EventStreamSerializationCompressionAttribute : Attribute
    {
        public EventStreamSerializationCompressionAttribute(Compressions compression,
            CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            Compression = compression;
            CompressionLevel = compressionLevel;
        }

        public Compressions Compression { get; }

        public CompressionLevel CompressionLevel { get; }
    }
}