using InventoryManagement.Infrastructure.DataBase.Configurations;
using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.DataBase;

public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GitHubThirdPartyConnection> GitHubThirdPartyConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ItemConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new CategoryConfiguration())
            .ApplyConfiguration(new GitHubThirdPartyConnectionConfiguration());
    }
}