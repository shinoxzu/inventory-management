using InventoryManagement.Application.DTO.Categories;
using InventoryManagement.Application.Errors;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace InventoryManagement.Tests;

public sealed class CategoriesServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task TestFetchingNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await categoriesService.GetCategory(Guid.Empty, Guid.Empty));
    }

    [Fact]
    public async Task TestCreatingCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        var newCategory = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "TestCategory",
            ParentId = null
        });

        var fetchedCategory = await categoriesService.GetCategory(userId, newCategory.Id);

        Assert.Equal(newCategory.Id, fetchedCategory.Id);
        Assert.Equal("TestCategory", fetchedCategory.Name);
    }

    [Fact]
    public async Task TestCreatingSubCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        var parentCategory = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Parent",
            ParentId = null
        });

        var childCategory = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Child",
            ParentId = parentCategory.Id
        });

        Assert.Equal(parentCategory.Id, childCategory.ParentId);
    }

    [Fact]
    public async Task TestDeletingCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        var category = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "ToDelete",
            ParentId = null
        });

        await categoriesService.RemoveCategory(userId, category.Id);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await categoriesService.GetCategory(userId, category.Id));
    }

    [Fact]
    public async Task TestUpdatingCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        var category = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Original",
            ParentId = null
        });

        await categoriesService.UpdateCategory(userId, category.Id, new CreateCategoryDTO
        {
            Name = "Updated",
            ParentId = null
        });

        var updatedCategory = await categoriesService.GetCategory(userId, category.Id);
        Assert.Equal("Updated", updatedCategory.Name);
    }

    [Fact]
    public async Task TestGettingUserCategories()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        var parentCategory = await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Parent",
            ParentId = null
        });

        await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Child1",
            ParentId = parentCategory.Id
        });

        await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Child2",
            ParentId = parentCategory.Id
        });

        var childCategories = await categoriesService.GetUserCategories(userId, new GetCategoriesDTO
        {
            ParentId = parentCategory.Id
        });

        Assert.Equal(2, childCategories.Categories.Count);
    }

    [Fact]
    public async Task TestCreatingCategoryWithNonExistentParent()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(async () =>
            await categoriesService.CreateCategory(userId, new CreateCategoryDTO
            {
                Name = "Invalid",
                ParentId = Guid.NewGuid()
            }));
    }

    [Fact]
    public async Task TestGettingRootCategories()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Root1",
            ParentId = null
        });

        await categoriesService.CreateCategory(userId, new CreateCategoryDTO
        {
            Name = "Root2",
            ParentId = null
        });

        var rootCategories = await categoriesService.GetUserCategories(userId, new GetCategoriesDTO
        {
            ParentId = null
        });

        Assert.Equal(2, rootCategories.Categories.Count);
    }

    [Fact]
    public async Task TestAccessDeniedForOtherUserCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);

        var category = await categoriesService.CreateCategory(user1Id, new CreateCategoryDTO
        {
            Name = "Category",
            ParentId = null
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await categoriesService.GetCategory(user2Id, category.Id));
    }

    [Fact]
    public async Task TestUpdateCategoryToOtherUserParent()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);

        var category1 = await categoriesService.CreateCategory(user1Id, new CreateCategoryDTO
        {
            Name = "Category1",
            ParentId = null
        });

        var category2 = await categoriesService.CreateCategory(user2Id, new CreateCategoryDTO
        {
            Name = "Category2",
            ParentId = null
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await categoriesService.UpdateCategory(user1Id, category1.Id, new CreateCategoryDTO
            {
                Name = "UpdatedCategory",
                ParentId = category2.Id
            }));
    }

    [Fact]
    public async Task TestGetCategoriesFromOtherUserParent()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);

        var parentCategory = await categoriesService.CreateCategory(user1Id, new CreateCategoryDTO
        {
            Name = "Parent",
            ParentId = null
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await categoriesService.GetUserCategories(user2Id, new GetCategoriesDTO
            {
                ParentId = parentCategory.Id
            }));
    }

    [Fact]
    public async Task TestUpdateNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await categoriesService.UpdateCategory(userId, Guid.NewGuid(), new CreateCategoryDTO
            {
                Name = "Updated",
                ParentId = null
            }));
    }

    [Fact]
    public async Task TestDeleteOtherUserCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);

        var category = await categoriesService.CreateCategory(user1Id, new CreateCategoryDTO
        {
            Name = "Category",
            ParentId = null
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await categoriesService.RemoveCategory(user2Id, category.Id));
    }

    [Fact]
    public async Task TestDeleteNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);
        var categoriesService = new CategoriesService(dbContext);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await categoriesService.RemoveCategory(userId, Guid.NewGuid()));
    }
}