using Microsoft.AspNetCore.Authentication;
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
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<EnvironmentController> _logger;

    public EnvironmentController(IEnvironmentRepository environmentRepository, IAuthenticationService authenticationService, ILogger<EnvironmentController> logger)
    {
        _environmentRepository = environmentRepository;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();


        var environments = await _environmentRepository.ReadByUserIdAsync(userId);
        return Ok(environments);
    }

    [HttpGet("{environmentId}", Name = "ReadEnvironment")]
    public async Task<ActionResult<Environment2D>> Get(Guid environmentId)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var environment = await _environmentRepository.ReadAsync(environmentId);
        if (environment == null || environment.OwnerUserId != userId)
            return NotFound($"Environment with id {environmentId} not found in your account.");

        return Ok(environment);
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult> Add(Environment2D environment)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var userEnvironments = await _environmentRepository.ReadByUserIdAsync(userId);
        if (userEnvironments.Count() >= Environment2D.MaxNumberOfEnvironments)
        {
            return BadRequest($"You cannot create more than {Environment2D.MaxNumberOfEnvironments} environments.");
        }

        if (userEnvironments.Any(e => e.Name == environment.Name))
        {
            return BadRequest("An environment with the same name already exists.");
        }

        environment.Id = Guid.NewGuid();
        environment.OwnerUserId = userId;

        var createdEnvironment = await _environmentRepository.InsertAsync(environment);
        return CreatedAtRoute("ReadEnvironment", new { environmentId = createdEnvironment.Id }, createdEnvironment);
    }

    [HttpPut("{environmentId}", Name = "UpdateEnvironment")]
    public async Task<ActionResult> Update(Guid environmentId, Environment2D newEnvironment)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        if (existingEnvironment.OwnerUserId != userId)
            return Unauthorized();

        var userEnvironments = await _environmentRepository.ReadByUserIdAsync(userId);
        if (userEnvironments.Any(e => e.Name == newEnvironment.Name && e.Id != environmentId))
        {
            return BadRequest("An environment with the same name already exists.");
        }

        newEnvironment.Id = environmentId;
        newEnvironment.OwnerUserId = userId;
        newEnvironment.Name = existingEnvironment.Name;
        await _environmentRepository.UpdateAsync(newEnvironment);

        return Ok(newEnvironment);
    }


    [HttpDelete("{environmentId}", Name = "DeleteEnvironment")]
    public async Task<IActionResult> Delete(Guid environmentId)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (userId == null)
            return Unauthorized();

        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);
        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        if (existingEnvironment.OwnerUserId != userId)
            return Unauthorized();

        await _environmentRepository.DeleteAsync(environmentId);

        return Ok();
    }
}
