using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
}
