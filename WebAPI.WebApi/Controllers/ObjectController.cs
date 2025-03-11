using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;

namespace ProjectLU2.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("environments")]
public class ObjectController : ControllerBase
{
    private readonly IObjectRepository _objectRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<ObjectController> _logger;

    public ObjectController(IObjectRepository objectRepository, IEnvironmentRepository environmentRepository, IAuthenticationService authenticationService, ILogger<ObjectController> logger)
    {
        _objectRepository = objectRepository;
        _environmentRepository = environmentRepository;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpGet("objects", Name = "ReadObjects")]
    public async Task<ActionResult<IEnumerable<Object2D>>> Get()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var objects = await _objectRepository.ReadAllAsync();
        return Ok(objects);
    }

    [HttpGet("objects/{objectId}", Name = "ReadObject")]
    public async Task<ActionResult<Object2D>> Get(Guid objectId)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var obj = await _objectRepository.ReadAsync(objectId);
        if (obj == null)
            return NotFound($"Object with id {objectId} not found.");

        var environment = await _environmentRepository.ReadAsync(obj.EnvironmentId);
        if (environment == null || environment.OwnerUserId != userId)
            return NotFound($"Object with id {objectId} not found in your account.");

        return Ok(obj);
    }

    [HttpGet("{environmentId}/objects", Name = "ReadObjectsByEnvironment")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetByEnvironment(Guid environmentId)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var environment = await _environmentRepository.ReadAsync(environmentId);
        if (environment == null || environment.OwnerUserId != userId)
            return NotFound($"Environment with id {environmentId} not found in your account.");

        var objects = await _objectRepository.ReadByEnvironmentIdAsync(environmentId);
        return Ok(objects);
    }

    [HttpPost("{environmentId}/objects", Name = "CreateObject")]
    public async Task<ActionResult<Object2D>> Add(Guid environmentId, Object2D obj)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var environment = await _environmentRepository.
            ReadAsync(environmentId);
        if (environment == null || environment.OwnerUserId != userId)
            return NotFound($"Environment with id {environmentId} not found in your account.");

        obj.Id = Guid.NewGuid();
        obj.EnvironmentId = environmentId;

        var createdObject = await _objectRepository.InsertAsync(obj);
        return CreatedAtRoute("ReadObject", new { objectId = createdObject.Id }, createdObject);
    }

    [HttpPut("{environmentId}/objects/{objectId}", Name = "UpdateObject")]
    public async Task<ActionResult> Update(Guid objectId, Object2D newObject)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var existingObject = await _objectRepository.ReadAsync(objectId);

        if (existingObject == null)
            return NotFound($"Object with id {objectId} not found.");

        var existingEnvironment = await _environmentRepository.ReadAsync(existingObject.EnvironmentId);
        if (existingEnvironment == null)
            return NotFound($"Environment with id {existingObject.EnvironmentId} not found.");

        if (existingEnvironment.OwnerUserId != userId)
            return Unauthorized();

        newObject.Id = objectId;
        newObject.EnvironmentId = existingObject.EnvironmentId;

        await _objectRepository.UpdateAsync(newObject);

        return Ok(newObject);
    }

    [HttpDelete("{environmentId}/objects/{objectId}", Name = "DeleteObject")]
    public async Task<IActionResult> Delete(Guid objectId)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var existingObject = await _objectRepository.ReadAsync(objectId);

        if (existingObject == null)
            return NotFound($"Object with id {objectId} not found.");

        var existingEnvironment = await _environmentRepository.ReadAsync(existingObject.EnvironmentId);
        if (existingEnvironment == null)
            return NotFound($"Environment with id {existingObject.EnvironmentId} not found.");

        if (existingEnvironment.OwnerUserId != userId)
            return Unauthorized();

        await _objectRepository.DeleteAsync(objectId);

        return Ok();
    }
}
