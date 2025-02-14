namespace InventoryManagement.Infrastructure.DataBase.Models;

public class GitHubThirdPartyConnection
{
    public required string Login { get; set; }
    public required User User { get; set; }
}