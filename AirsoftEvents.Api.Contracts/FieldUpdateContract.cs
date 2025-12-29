namespace AirsoftEvents.Api.Contracts;

public class FieldUpdateContract
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Capacity { get; set; }
    public string Address { get; set; } = default!;
}