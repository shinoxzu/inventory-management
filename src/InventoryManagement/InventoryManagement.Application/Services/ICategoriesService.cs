using InventoryManagement.Application.DTO.Categories;

namespace InventoryManagement.Application.Services;

public interface ICategoriesService
{
    Task<CategoryDTO> GetCategory(Guid fromUserId, Guid categoryId);

    Task<GetCategoriesResponseDTO> GetUserCategories(Guid fromUserId, GetCategoriesDTO getCategoriesDto);

    Task<CategoryDTO> CreateCategory(Guid fromUserId, CreateCategoryDTO createCategoryDto);

    Task RemoveCategory(Guid fromUserId, Guid categoryId);

    Task UpdateCategory(Guid fromUserId, Guid categoryId, CreateCategoryDTO createCategoryDto);
}