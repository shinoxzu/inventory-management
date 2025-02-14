using InventoryManagement.Application.DTO.Auth;

namespace InventoryManagement.Application.Services;

public interface IGitHubAuthService
{
    Task<AuthorizedUserDTO> LoginUserWithOauthCode(string code);
}