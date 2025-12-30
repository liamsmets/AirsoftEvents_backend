namespace AirsoftEvents.Api.Options;

public class MollieOptions
{
    public string WebhookSecret { get; set; } = "";
    public string RedirectBaseUrl { get; set; } = "http://localhost:5173";
    public string BackendBaseUrl { get; set; } = "http://localhost:5187";
}
