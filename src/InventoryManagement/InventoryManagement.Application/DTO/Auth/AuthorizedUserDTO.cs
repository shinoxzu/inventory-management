using InventoryManagement.Application.DTO.Users;

namespace InventoryManagement.Application.DTO.Auth;

public class AuthorizedUserDTO
{
    public required string Token { get; set; }
    public required UserDTO User { get; set; }
}