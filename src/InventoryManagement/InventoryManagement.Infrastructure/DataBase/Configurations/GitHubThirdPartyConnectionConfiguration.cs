using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.DataBase.Configurations;

public class GitHubThirdPartyConnectionConfiguration : IEntityTypeConfiguration<GitHubThirdPartyConnection>
{
    public void Configure(EntityTypeBuilder<GitHubThirdPartyConnection> builder)
    {
        builder.HasKey(g => g.Login);

        builder.HasOne<User>(g => g.User)
            .WithMany()
            .IsRequired();
    }
}