namespace AirsoftEvents.Core.Exceptions;

public class CapacityExceededException : Exception
{
    public CapacityExceededException(string message) : base(message)
    {
    }
}