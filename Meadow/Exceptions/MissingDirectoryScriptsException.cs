using System;
using System.Runtime.Serialization;

namespace Meadow.Exceptions
{
    public class MissingDirectoryScriptsException:Exception
    {
        public MissingDirectoryScriptsException():this("Could not find the directory containing your scripts.")
        {
            
        }

        protected MissingDirectoryScriptsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingDirectoryScriptsException(string message) : base(message)
        {
        }

        public MissingDirectoryScriptsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}