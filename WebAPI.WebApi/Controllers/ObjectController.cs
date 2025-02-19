using Microsoft.AspNetCore.Mvc;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;

namespace ProjectLU2.WebApi.Controllers;

[ApiController]
[Route("environments")]
public class ObjectController : ControllerBase
{
    private readonly IObjectRepository _objectRepository;
    private readonly ILogger<ObjectController> _logger;

    public ObjectController(IObjectRepository objectRepository, ILogger<ObjectController> logger)
    {
        _objectRepository = objectRepository;
        _logger = logger;
    }

    [HttpGet("{environmentId:guid}/[controller]", Name = "ReadObjects")]
    public async Task<ActionResult<IEnumerable<Object2D>>> Get()
    {
        var objects = await _objectRepository.ReadAllAsync();
        return Ok(objects);
    }

    [HttpGet("{environmentId:guid}/[controller]/{objectId:guid}", Name = "ReadObject")]
    public async Task<ActionResult<Object2D>> Get(Guid objectId)
    {
        var objects = await _objectRepository.ReadAsync(objectId);
        if (objects == null)
            return NotFound($"Object with id {objectId} not found.");

        return Ok(objects);
    }

    [HttpGet("{environmentId:guid}", Name = "ReadObjectsByEnvironment")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetByEnvironment(Guid environmentId)
    {
        var objects = await _objectRepository.ReadByEnvironmentIdAsync(environmentId);
        return Ok(objects);
    }

    [HttpPost("{environmentId:guid}/[controller]", Name = "CreateObject")]
    public async Task<ActionResult<Object2D>> Add(Guid environmentId, Object2D obj)
    {
        obj.Id = Guid.NewGuid();
        obj.EnvironmentId = environmentId;

        var createdObject = await _objectRepository.InsertAsync(obj);
        return CreatedAtRoute("ReadObject", new { objectId = createdObject.Id }, createdObject);
    }

    [HttpPut("{environmentId:guid}/[controller]/{objectId:guid}", Name = "UpdateObject")]
    public async Task<ActionResult> Update(Guid objectId, Object2D newObject)
    {
        var existingObject = await _objectRepository.ReadAsync(objectId);

        if (existingObject == null)
            return NotFound($"Object with id {objectId} not found.");

        await _objectRepository.UpdateAsync(newObject);

        return Ok(newObject);
    }

    [HttpDelete("{environmentId:guid}/[controller]/{objectId:guid}", Name = "DeleteObject")]
    public async Task<IActionResult> Delete(Guid objectId)
    {
        var existingObject = await _objectRepository.ReadAsync(objectId);

        if (existingObject == null)
            return NotFound($"Object with id {objectId} not found.");

        await _objectRepository.DeleteAsync(objectId);

        return Ok();
    }
}
