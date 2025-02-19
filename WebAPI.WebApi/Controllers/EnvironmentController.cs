using Microsoft.AspNetCore.Mvc;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;

namespace ProjectLU2.WebApi.Controllers;

[ApiController]
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
        var environments = await _environmentRepository.ReadAsync();
        return Ok(environments);
    }

    [HttpGet("{environmentId:guid}", Name = "ReadEnvironment")]
    public async Task<ActionResult<Environment2D>> Get(Guid environmentId)
    {
        var environment = await _environmentRepository.ReadAsync(environmentId);
        if (environment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        return Ok(environment);
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult> Add(Environment2D environment)
    {
        environment.Id = Guid.NewGuid();

        var createdEnvironment = await _environmentRepository.InsertAsync(environment);
        return CreatedAtRoute("ReadEnvironment", new { environmentId = createdEnvironment.Id }, createdEnvironment);
    }

    [HttpPut("{environmentId:guid}", Name = "UpdateEnvironment")]
    public async Task<ActionResult> Update(Guid environmentId, Environment2D newEnvironment)
    {
        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        await _environmentRepository.UpdateAsync(newEnvironment);

        return Ok(newEnvironment);
    }

    [HttpDelete("{environmentId:guid}", Name = "DeleteEnvironment")]
    public async Task<IActionResult> Delete(Guid environmentId)
    {
        var existingEnvironment = await _environmentRepository.ReadAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound($"Environment with id {environmentId} not found.");

        await _environmentRepository.DeleteAsync(environmentId);

        return Ok();
    }
}
