namespace InventoryManagement.Infrastructure.DataBase.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
}