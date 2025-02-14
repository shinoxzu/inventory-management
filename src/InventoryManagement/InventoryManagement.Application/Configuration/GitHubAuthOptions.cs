namespace InventoryManagement.Application.Configuration;

public class GitHubAuthOptions
{
    public const string GitHubAuth = "ThirdPartyAuthorizations:GitHubAuth";

    public required string AppName { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}