using InventoryManagement.Application.DTO.Auth;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IGitHubAuthService gitHubAuthService) : ControllerBase
{
    [HttpGet("github")]
    public async Task<ActionResult<AuthorizedUserDTO>> LoginWithGithub([FromQuery] string code)
    {
        var authorizedUser = await gitHubAuthService.LoginUserWithOauthCode(code);
        return Ok(authorizedUser);
    }
}