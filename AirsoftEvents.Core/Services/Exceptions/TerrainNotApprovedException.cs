namespace AirsoftEvents.Core.Exceptions;

public class TerrainNotApprovedException : Exception
{
    public TerrainNotApprovedException(string message) : base(message)
    {
    }
}