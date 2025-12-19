namespace AirsoftEvents.Domain.Services.Exceptions;

public class TerrainNotApprovedException : Exception
{
    public TerrainNotApprovedException(string message) : base(message)
    {
    }
}