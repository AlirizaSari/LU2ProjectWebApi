using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;
using System.Security.Claims;

namespace ProjectLU2.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("environments")]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ILogger<EnvironmentController> _logger;

    public EnvironmentController(IEnvironmentRepository environmentRepository, ILogger<EnvironmentController> logger)
    {
        _environmentRepository = environmentRepository;
        _logger = logger;
    }

    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var environments = await _environmentRepository.ReadAllAsync();
        return Ok(environments);
    }

    [HttpGet("{environmentId}", Name = "ReadEnvironment")]
    public async Task<ActionResult<Environment2D>> Get(Guid environmentId)
    {
        var environment = await _environmentRepository.ReadAsync(environmentId);
        if (environment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        return Ok(environment);
    }

    // Read all environments for the current user
    [HttpGet("user/{ownerUserId}", Name = "ReadUserEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> GetUserEnvironments()
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerUserId == null)
        {
            return BadRequest($"Environment with user id {ownerUserId} not found.");
        }
        var environments = await _environmentRepository.ReadByUserIdAsync(ownerUserId);
        return Ok(environments);
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult> Add(Environment2D environment)
    {
        environment.Id = Guid.NewGuid();
        environment.OwnerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var createdEnvironment = await _environmentRepository.InsertAsync(environment);
        return CreatedAtRoute("ReadEnvironment", new { environmentId = createdEnvironment.Id }, createdEnvironment);
    }

    [HttpPut("{environmentId}", Name = "UpdateEnvironment")]
    public async Task<ActionResult> Update(Guid environmentId, Environment2D newEnvironment)
    {
        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        newEnvironment.Id = environmentId;
        newEnvironment.OwnerUserId = existingEnvironment.OwnerUserId;
        await _environmentRepository.UpdateAsync(newEnvironment);

        return Ok(newEnvironment);
    }


    [HttpDelete("{environmentId}", Name = "DeleteEnvironment")]
    public async Task<IActionResult> Delete(Guid environmentId)
    {
        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        await _environmentRepository.DeleteAsync(environmentId);

        return Ok();
    }
}
