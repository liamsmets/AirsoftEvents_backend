using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AirsoftEvents.Api.Extensions;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("fields")]
public class FieldsController : ControllerBase
{
    private readonly IFieldService _service;

    public FieldsController(IFieldService service)
    {
        _service = service;
    }

    [Authorize(Policy = "ApiWritePolicy")]
    [HttpPost]
    public async Task<IActionResult> CreateField([FromBody] FieldRequestContract fieldToCreate)
    {
        var ownerId = User.GetUserId();

        try
        {
            var createdField = await _service.CreateFieldAsync(fieldToCreate, ownerId);
            return CreatedAtAction(nameof(GetFieldById), new { id = createdField.Id }, createdField);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllFields()
    {
        var fields = await _service.GetAllFieldsAsync();
        return Ok(fields);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFieldById([FromRoute] Guid id)
    {
        var field = await _service.GetFieldByIdAsync(id);

        if (field == null)
        {
            return NotFound();
        }

        return Ok(field);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("Approved")]
    public async Task<IActionResult> GetApprovedFields()
    {
        var field = await _service.GetApprovedFieldsAsync();

        if (field == null)
        {
            return NotFound();
        }

        return Ok(field);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetFieldByOwnerId()
    {
        var ownerId = User.GetUserId();

        var fields = await _service.GetFieldByOwnerIdAsync(ownerId);
        return Ok(fields);
    }

    [Authorize(Policy = "ApiAdminPolicy")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveField([FromRoute] Guid id)
    {
        try
        {
            await _service.ApproveFieldAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}