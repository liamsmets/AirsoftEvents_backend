namespace AirsoftEvents.Api.Options;

public class MollieOptions
{
    public string WebhookSecret { get; set; } = "";
    public string RedirectBaseUrl { get; set; } = "";
    public string BackendBaseUrl { get; set; } = "";
}
