namespace InventoryManagement.Application.Configuration;

public class JWTOptions
{
    public const string JWT = "JWT";

    public required string Audience { get; set; }
    public required string Issuer { get; set; }
    public required string Key { get; set; }
}