namespace InventoryManagement.Application.DTO.Categories;

public class CategoryDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid? ParentId { get; set; }
}