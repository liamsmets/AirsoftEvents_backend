namespace AirsoftEvents.Api.Contracts;

public class EventUpdateContract
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int MaxPlayers { get; set; }
    public Guid FieldId { get; set; }
}
