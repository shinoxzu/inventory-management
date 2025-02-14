using InventoryManagement.Application.Configuration;
using Octokit;

namespace InventoryManagement.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubClient(this IServiceCollection serviceCollection, string appName)
    {
        var client = new GitHubClient(new ProductHeaderValue(appName));
        return serviceCollection.AddSingleton(client);
    }

    public static IServiceCollection ConfigureOptionsFromConfiguration(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection
            .Configure<GitHubAuthOptions>(configuration.GetSection(GitHubAuthOptions.GitHubAuth))
            .Configure<JWTOptions>(configuration.GetSection(JWTOptions.JWT));
    }
}