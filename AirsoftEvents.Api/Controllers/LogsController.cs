using AirsoftEvents.Persistance.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("logs")]
public class LogsController(ILogRepo logRepo) : ControllerBase
{
    [Authorize(Policy = "ApiAdminPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLog([FromRoute] string id)
    {
        var log = await logRepo.GetAsync(id);
        if (log is null) return NotFound();
        return Ok(log);
    }
}
