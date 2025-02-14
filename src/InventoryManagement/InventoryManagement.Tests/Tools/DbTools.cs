using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.DataBase.Models;

namespace InventoryManagement.Tests.Tools;

public static class DbTools
{
    public static async Task<Guid> AddUserToDb(DataBaseContext dbContext)
    {
        var user = new User
        {
            Name = "TestUser",
            AvatarUrl = null
        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public static async Task<Guid> AddCategoryToDb(DataBaseContext dbContext, Guid authorId)
    {
        var category = new Category
        {
            Name = "TestUser",
            ParentId = null,
            AuthorId = authorId
        };
        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        return category.Id;
    }
}