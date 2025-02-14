namespace InventoryManagement.Application.DTO.Categories;

public class CreateCategoryDTO
{
    public required string Name { get; set; }
    public Guid? ParentId { get; set; }
}