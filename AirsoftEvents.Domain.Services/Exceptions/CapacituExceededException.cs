namespace AirsoftEvents.Domain.Services.Exceptions;

public class CapacityExceededException : Exception
{
    public CapacityExceededException(string message) : base(message)
    {
    }
}