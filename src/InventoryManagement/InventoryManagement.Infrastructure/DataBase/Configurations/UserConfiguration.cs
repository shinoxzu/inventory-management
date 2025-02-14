using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.DataBase.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(256);

        builder.HasMany<Category>()
            .WithOne()
            .HasForeignKey(c => c.AuthorId)
            .IsRequired();

        builder.HasMany<Item>()
            .WithOne()
            .HasForeignKey(i => i.AuthorId)
            .IsRequired();

        builder.HasMany<GitHubThirdPartyConnection>()
            .WithOne(gc => gc.User)
            .IsRequired();
    }
}