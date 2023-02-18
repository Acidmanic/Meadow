using System;

namespace Meadow.Exceptions;

public class IncompatibleCodeGeneratorException:Exception
{
    public IncompatibleCodeGeneratorException(string message):base(message)
    {
        
    }
}