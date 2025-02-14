using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.Infrastructure.Tools;

public static class SecurityTools
{
    public static SymmetricSecurityKey GetSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}