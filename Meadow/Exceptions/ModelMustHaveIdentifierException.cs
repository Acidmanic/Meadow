using System;
using System.Runtime.Serialization;

namespace Meadow.Exceptions
{
    public class ModelMustHaveIdentifierException:Exception
    {
        public ModelMustHaveIdentifierException():base("Model must have one unique identifier property.")
        {
        }

        protected ModelMustHaveIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ModelMustHaveIdentifierException(string message) : base(message)
        {
        }

        public ModelMustHaveIdentifierException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}