namespace InventoryManagement.Application.DTO.Categories;

public class GetCategoriesDTO
{
    public required Guid? ParentId { get; set; }
}