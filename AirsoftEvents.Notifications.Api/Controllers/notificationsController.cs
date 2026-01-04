using Microsoft.AspNetCore.Mvc;

namespace AirsoftEvents.NotificationService.Api.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    [HttpPost("email")]
    public IActionResult SendEmail([FromBody] EmailNotificationDto dto)
    {
        Console.WriteLine("📧 Fake email sent");
        Console.WriteLine($"To: {dto.To}");
        Console.WriteLine($"Subject: {dto.Subject}");
        Console.WriteLine($"Body: {dto.Body}");

        return Accepted(new
        {
            id = Guid.NewGuid(),
            status = "queued"
        });
    }
}

public record EmailNotificationDto(
    string To,
    string Subject,
    string Body
);
