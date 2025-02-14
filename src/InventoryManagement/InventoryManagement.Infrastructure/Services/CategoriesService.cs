using InventoryManagement.Application.DTO.Categories;
using InventoryManagement.Application.Errors;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Services;

public class CategoriesService(DataBaseContext dataBaseContext) : ICategoriesService
{
    public async Task<CategoryDTO> GetCategory(Guid fromUserId, Guid categoryId)
    {
        var category = await dataBaseContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category is null) throw new NotFoundError("Category with such id not found.");
        if (category.AuthorId != fromUserId)
            throw new AccessDeniedError("You cannot fetch this category, it's not yours.");

        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId
        };
    }

    public async Task<GetCategoriesResponseDTO> GetUserCategories(
        Guid fromUserId,
        GetCategoriesDTO getCategoriesDto)
    {
        if (getCategoriesDto.ParentId is not null) await GetCategory(fromUserId, getCategoriesDto.ParentId.Value);

        var categories = await dataBaseContext.Categories
            .Where(c => c.ParentId == getCategoriesDto.ParentId)
            .Where(c => c.AuthorId == fromUserId)
            .ToListAsync();

        var categoriesTotalCount = await dataBaseContext.Items
            .Where(i => i.AuthorId == fromUserId)
            .CountAsync();

        return new GetCategoriesResponseDTO
        {
            Categories = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Name = c.Name
            }).ToList(),
            TotalCount = categoriesTotalCount
        };
    }

    public async Task<CategoryDTO> CreateCategory(Guid fromUserId, CreateCategoryDTO createCategoryDto)
    {
        if (createCategoryDto.ParentId is not null) await GetCategory(fromUserId, createCategoryDto.ParentId.Value);

        var category = new Category
        {
            Name = createCategoryDto.Name,
            ParentId = createCategoryDto.ParentId,
            AuthorId = fromUserId
        };
        await dataBaseContext.Categories.AddAsync(category);
        await dataBaseContext.SaveChangesAsync();

        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId
        };
    }

    public async Task RemoveCategory(Guid fromUserId, Guid categoryId)
    {
        var category = await dataBaseContext.Categories.FindAsync(categoryId);

        if (category is null) throw new NotFoundError("Category witch such id cannot be found.");
        if (category.AuthorId != fromUserId)
            throw new AccessDeniedError("You cannot remove this category, it's not yours.");

        dataBaseContext.Categories.Remove(category);
        await dataBaseContext.SaveChangesAsync();
    }

    public async Task UpdateCategory(Guid fromUserId, Guid categoryId, CreateCategoryDTO createCategoryDto)
    {
        var category = await dataBaseContext.Categories.FindAsync(categoryId);

        if (category is null) throw new NotFoundError("Category witch such id cannot be found.");
        if (category.AuthorId != fromUserId)
            throw new AccessDeniedError("You cannot update this category, it's not yours.");

        if (createCategoryDto.ParentId is not null) await GetCategory(fromUserId, createCategoryDto.ParentId.Value);

        category.Name = createCategoryDto.Name;
        category.ParentId = createCategoryDto.ParentId;

        await dataBaseContext.SaveChangesAsync();
    }
}