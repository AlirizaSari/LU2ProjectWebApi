using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.WebApi.Models;

[ApiController]
[Route("account/[controller]")]
public class AccountManagementController : ControllerBase
{
    private readonly ILogger<AccountManagementController> logger;
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly RoleManager<IdentityUser> roleManager;

    public AccountManagementController(ILogger<AccountManagementController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityUser> roleManager)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
    }

    [HttpPost("{id}/{claim}/{value?}", Name = "AddClaimToUser")]
    public async Task<ActionResult<User>> AddClaimToUser(string id, string claim, string value = null!)
    {
        if (id is null)
        {
            return BadRequest("Id is required");
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var result = await userManager.AddClaimAsync(user, new Claim(claim, value));
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}/{claim}/{value?}", Name = "RemoveClaimFromUser")]
    public async Task<ActionResult<User>> RemoveClaimFromUser(string id, string claim, string value = null!)
    {
        if (id is null)
        {
            return BadRequest("Id is required");
        }
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var result = await userManager.RemoveClaimAsync(user, new Claim(claim, value));
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }
}
