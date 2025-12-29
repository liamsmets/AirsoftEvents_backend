using AirsoftEvents.Domain.Models.Enums;
namespace AirsoftEvents.Api.Contracts;

public class FieldResponseContract
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int Capacity {get;set;}
    public required FieldStatus Status {get;set;}
    public Guid OwnerId{get;set;}
    public string? ImageUrl {get;set;}
} 