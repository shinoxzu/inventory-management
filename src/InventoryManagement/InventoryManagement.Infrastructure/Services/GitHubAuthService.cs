using InventoryManagement.Application.Configuration;
using InventoryManagement.Application.DTO.Auth;
using InventoryManagement.Application.DTO.Users;
using InventoryManagement.Application.Errors;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Octokit;
using User = InventoryManagement.Infrastructure.DataBase.Models.User;

namespace InventoryManagement.Infrastructure.Services;

public class GitHubAuthService(
    GitHubClient client,
    DataBaseContext dataBaseContext,
    IOptions<GitHubAuthOptions> githubAuthOptions,
    ITokenService tokenService) : IGitHubAuthService
{
    public async Task<AuthorizedUserDTO> LoginUserWithOauthCode(string code)
    {
        var request = new OauthTokenRequest(
            githubAuthOptions.Value.ClientId,
            githubAuthOptions.Value.ClientSecret,
            code);

        var githubToken = await client.Oauth.CreateAccessToken(request);
        if (githubToken?.AccessToken is null) throw new ProvidedDataIsInvalidError("Code is incorrect.");

        var userClient = new GitHubClient(new ProductHeaderValue(githubAuthOptions.Value.AppName))
        {
            Credentials = new Credentials(githubToken.AccessToken)
        };
        var githubUser = await userClient.User.Current();

        var githubThirdPartyConnection = await dataBaseContext.GitHubThirdPartyConnections
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Login == githubUser.Login);

        if (githubThirdPartyConnection is null)
        {
            var user = new User
            {
                Name = githubUser.Name,
                AvatarUrl = null
            };
            await dataBaseContext.Users.AddAsync(user);
            await dataBaseContext.SaveChangesAsync();

            githubThirdPartyConnection = new GitHubThirdPartyConnection
            {
                Login = githubUser.Login,
                User = user
            };
            await dataBaseContext.GitHubThirdPartyConnections.AddAsync(githubThirdPartyConnection);
            await dataBaseContext.SaveChangesAsync();
        }

        var token = tokenService.GenerateToken(githubThirdPartyConnection.User.Id);
        return new AuthorizedUserDTO
        {
            Token = token,
            User = new UserDTO
            {
                Id = githubThirdPartyConnection.User.Id,
                AvatarUrl = githubThirdPartyConnection.User.AvatarUrl,
                Name = githubThirdPartyConnection.User.Name
            }
        };
    }
}