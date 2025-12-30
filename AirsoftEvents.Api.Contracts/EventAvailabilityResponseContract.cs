namespace AirsoftEvents.Api.Contracts;

public class EventAvailabilityResponseContract
{
    public Guid EventId { get; set; }
    public int MaxPlayers { get; set; }
    public int Reserved { get; set; }
    public int Free { get; set; }
}
