using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Services.Interfaces;

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

    [HttpPost]
    public async Task<IActionResult> CreateField([FromBody] FieldRequestContract fieldToCreate)
    {
        var createdField = await _service.CreateFieldAsync(fieldToCreate);
        
        return CreatedAtAction(nameof(GetFieldById), new { id = createdField.Id }, createdField);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFields()
    {
        var fields = await _service.GetAllFieldsAsync();
        return Ok(fields);
    }

    
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
}