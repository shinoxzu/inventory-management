namespace InventoryManagement.Application.DTO.Users;

public class UserDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
}