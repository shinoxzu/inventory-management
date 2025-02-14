namespace InventoryManagement.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId);
}