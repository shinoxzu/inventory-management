using InventoryManagement.Application.DTO.Items;
using InventoryManagement.Application.Errors;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Services;

public class ItemsService(DataBaseContext dataBaseContext, ICategoriesService categoriesService) : IItemsService
{
    public async Task<ItemDTO> GetItem(Guid fromUserId, Guid itemId)
    {
        var item = await dataBaseContext.Items
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item is null) throw new NotFoundError("Item witch such id not found.");
        if (item.AuthorId != fromUserId) throw new AccessDeniedError("You cannot fetch this item, it's not yours.");

        return new ItemDTO
        {
            Id = item.Id,
            Name = item.Name,
            Count = item.Count,
            AuthorId = item.AuthorId,
            CategoryId = item.CategoryId
        };
    }

    public async Task<GetItemsResponseDTO> GetUserItems(Guid fromUserId, GetItemsDTO getItemsDto)
    {
        List<Item> items;

        if (getItemsDto.CategoryId is null)
        {
            items = await dataBaseContext.Items
                .Where(i => i.AuthorId == fromUserId)
                .ToListAsync();
        }
        else
        {
            await categoriesService.GetCategory(fromUserId, getItemsDto.CategoryId.Value);

            items = await dataBaseContext.Items
                .Where(i => i.AuthorId == fromUserId && i.CategoryId == getItemsDto.CategoryId)
                .ToListAsync();
        }

        var itemsTotalCount = await dataBaseContext.Items
            .Where(i => i.AuthorId == fromUserId)
            .CountAsync();

        return new GetItemsResponseDTO
        {
            Items = items.Select(item => new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Count = item.Count,
                AuthorId = item.AuthorId,
                CategoryId = item.CategoryId
            }).ToList(),
            TotalCount = itemsTotalCount
        };
    }

    public async Task<ItemDTO> CreateItem(Guid fromUserId, CreateItemDTO createItemDto)
    {
        var category = await dataBaseContext.Categories.FindAsync(createItemDto.CategoryId);
        if (category is null) throw new NotFoundError("Cannot find this category.");

        await categoriesService.GetCategory(fromUserId, createItemDto.CategoryId);

        var newItem = new Item
        {
            Name = createItemDto.Name,
            Count = createItemDto.Count,
            CategoryId = category.Id,
            AuthorId = fromUserId
        };
        dataBaseContext.Items.Add(newItem);
        await dataBaseContext.SaveChangesAsync();

        return new ItemDTO
        {
            Id = newItem.Id,
            Name = newItem.Name,
            Count = newItem.Count,
            AuthorId = newItem.AuthorId,
            CategoryId = newItem.CategoryId
        };
    }

    public async Task RemoveItem(Guid fromUserId, Guid itemId)
    {
        var item = await dataBaseContext.Items.FindAsync(itemId);

        if (item is null) throw new NotFoundError("Item witch such id cannot be found.");
        if (item.AuthorId != fromUserId) throw new AccessDeniedError("You cannot remove this item, it's not yours.");

        dataBaseContext.Items.Remove(item);
        await dataBaseContext.SaveChangesAsync();
    }

    public async Task UpdateItem(Guid fromUserId, Guid itemId, CreateItemDTO createItemDto)
    {
        var item = await dataBaseContext.Items.FindAsync(itemId);

        if (item is null) throw new NotFoundError("Item witch such id cannot be found.");
        if (item.AuthorId != fromUserId) throw new AccessDeniedError("You cannot update this item, it's not yours.");

        await categoriesService.GetCategory(fromUserId, createItemDto.CategoryId);

        item.Name = createItemDto.Name;
        item.CategoryId = createItemDto.CategoryId;
        item.Count = createItemDto.Count;

        await dataBaseContext.SaveChangesAsync();
    }
}