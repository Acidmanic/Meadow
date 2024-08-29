using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Extensions;

namespace Meadow.Attributes
{
    public class EventStreamSerializationEncodingAttribute : Attribute
    {
        
        private static readonly Dictionary<string, Encoding> EncodingsByName;

        static EventStreamSerializationEncodingAttribute()
        {
            EncodingsByName = new Dictionary<string, Encoding>();

            Action<Encoding> add = e => EncodingsByName.Add(e.EncodingName.ToLower(), e);

            add(Encoding.ASCII);
            add(Encoding.Unicode);
            add(Encoding.UTF8);
            add(Encoding.UTF32);
            add(Encoding.BigEndianUnicode);
            add(Encoding.Latin1);
        }


        public EventStreamSerializationEncodingAttribute(string? encoding)
        {
            Encoding = Find(encoding);
        }

        private Encoding Find(string? encoding)
        {
            if (encoding is { } name)
            {
                name = name.ToLower();

                if (EncodingsByName.ContainsKey(name))
                {
                    return EncodingsByName[name];
                }
            }
            
            return Encoding.Default;
        }
        
        public Encoding Encoding { get; }

        
    }
}