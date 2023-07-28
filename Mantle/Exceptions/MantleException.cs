using System.Runtime.Serialization;

namespace Mantle.Exceptions;

public class MantleException : Exception
{
    public MantleException()
    {
    }

    public MantleException(string message)
        : base(message)
    {
    }

    public MantleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected MantleException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}