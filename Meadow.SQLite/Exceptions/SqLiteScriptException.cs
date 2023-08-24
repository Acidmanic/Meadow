using System;
using System.Runtime.Serialization;

namespace Meadow.SQLite.Exceptions
{
    public class SqLiteScriptException:Exception
    {
        public SqLiteScriptException()
        {
        }

        protected SqLiteScriptException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SqLiteScriptException(string message) : base(message)
        {
        }

        public SqLiteScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}