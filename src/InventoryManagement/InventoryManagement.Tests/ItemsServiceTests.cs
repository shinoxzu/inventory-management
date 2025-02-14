using InventoryManagement.Application.DTO.Items;
using InventoryManagement.Application.Errors;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Tests.Tools;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace InventoryManagement.Tests;

public sealed class ItemsServiceTests : IAsyncLifetime
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
    public async Task TestFetchingNonExistentItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.GetItem(Guid.Empty, Guid.Empty));
    }

    [Fact]
    public async Task TestCreatingItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        var newItem = await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "SomeName",
            CategoryId = categoryId
        });
        var fetchedNewItem = await itemsService.GetItem(userId, newItem.Id);

        Assert.Equal(fetchedNewItem.Id, newItem.Id);
    }

    [Fact]
    public async Task TestDeletingItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        var newItem = await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "ItemToDelete",
            CategoryId = categoryId
        });

        await itemsService.RemoveItem(userId, newItem.Id);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.GetItem(userId, newItem.Id));
    }

    [Fact]
    public async Task TestUpdatingItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        var newItem = await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "OriginalName",
            CategoryId = categoryId
        });

        var updateDto = new CreateItemDTO
        {
            Count = 2,
            Name = "UpdatedName",
            CategoryId = categoryId
        };

        await itemsService.UpdateItem(userId, newItem.Id, updateDto);
        var updatedItem = await itemsService.GetItem(userId, newItem.Id);

        Assert.Equal("UpdatedName", updatedItem.Name);
        Assert.Equal(2, updatedItem.Count);
    }

    [Fact]
    public async Task TestGettingUserItems()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "Item1",
            CategoryId = categoryId
        });

        await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "Item2",
            CategoryId = categoryId
        });

        var items = await itemsService.GetUserItems(userId, new GetItemsDTO
        {
            CategoryId = categoryId
        });

        Assert.Equal(2, items.Items.Count);
        Assert.Equal(2, items.TotalCount);
    }

    [Fact]
    public async Task TestAccessDeniedForOtherUserItems()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId1 = await DbTools.AddUserToDb(dbContext);
        var userId2 = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId1);

        var newItem = await itemsService.CreateItem(userId1, new CreateItemDTO
        {
            Count = 1,
            Name = "SomeItem",
            CategoryId = categoryId
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await itemsService.GetItem(userId2, newItem.Id));
    }

    [Fact]
    public async Task TestGetItemsWithNoCategoryFilter()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var category1Id = await DbTools.AddCategoryToDb(dbContext, userId);
        var category2Id = await DbTools.AddCategoryToDb(dbContext, userId);

        await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "Item1",
            CategoryId = category1Id
        });

        await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "Item2",
            CategoryId = category2Id
        });

        var items = await itemsService.GetUserItems(userId, new GetItemsDTO
        {
            CategoryId = null
        });

        Assert.Equal(2, items.Items.Count);
    }

    [Fact]
    public async Task TestCreateItemInOtherUserCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, user1Id);

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await itemsService.CreateItem(user2Id, new CreateItemDTO
            {
                Count = 1,
                Name = "Item",
                CategoryId = categoryId
            }));
    }

    [Fact]
    public async Task TestUpdateItemInOtherUserCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);
        var category1Id = await DbTools.AddCategoryToDb(dbContext, user1Id);
        var category2Id = await DbTools.AddCategoryToDb(dbContext, user2Id);

        var newItem = await itemsService.CreateItem(user1Id, new CreateItemDTO
        {
            Count = 1,
            Name = "Item",
            CategoryId = category1Id
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await itemsService.UpdateItem(user1Id, newItem.Id, new CreateItemDTO
            {
                Count = 1,
                Name = "UpdatedItem",
                CategoryId = category2Id
            }));
    }

    [Fact]
    public async Task TestDeleteOtherUserItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, user1Id);

        var newItem = await itemsService.CreateItem(user1Id, new CreateItemDTO
        {
            Count = 1,
            Name = "Item",
            CategoryId = categoryId
        });

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await itemsService.RemoveItem(user2Id, newItem.Id));
    }

    [Fact]
    public async Task TestCreateItemWithNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.CreateItem(userId, new CreateItemDTO
            {
                Count = 1,
                Name = "Item",
                CategoryId = Guid.NewGuid()
            }));
    }

    [Fact]
    public async Task TestUpdateItemWithNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        var newItem = await itemsService.CreateItem(userId, new CreateItemDTO
        {
            Count = 1,
            Name = "Item",
            CategoryId = categoryId
        });

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.UpdateItem(userId, newItem.Id, new CreateItemDTO
            {
                Count = 1,
                Name = "UpdatedItem",
                CategoryId = Guid.NewGuid()
            }));
    }

    [Fact]
    public async Task TestUpdateNonExistentItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, userId);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.UpdateItem(userId, Guid.NewGuid(), new CreateItemDTO
            {
                Count = 1,
                Name = "Item",
                CategoryId = categoryId
            }));
    }

    [Fact]
    public async Task TestDeleteNonExistentItem()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.RemoveItem(userId, Guid.NewGuid()));
    }

    [Fact]
    public async Task TestGetItemsFromOtherUserCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var user1Id = await DbTools.AddUserToDb(dbContext);
        var user2Id = await DbTools.AddUserToDb(dbContext);
        var categoryId = await DbTools.AddCategoryToDb(dbContext, user1Id);

        await Assert.ThrowsAsync<AccessDeniedError>(
            async () => await itemsService.GetUserItems(user2Id, new GetItemsDTO
            {
                CategoryId = categoryId
            }));
    }

    [Fact]
    public async Task TestGetItemsFromNonExistentCategory()
    {
        var dbContext = new DataBaseContext(
            new DbContextOptionsBuilder<DataBaseContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options);

        var categoriesService = new CategoriesService(dbContext);
        var itemsService = new ItemsService(dbContext, categoriesService);

        var userId = await DbTools.AddUserToDb(dbContext);

        await Assert.ThrowsAsync<NotFoundError>(
            async () => await itemsService.GetUserItems(userId, new GetItemsDTO
            {
                CategoryId = Guid.NewGuid()
            }));
    }
}