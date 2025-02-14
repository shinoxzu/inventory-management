namespace InventoryManagement.Application.DTO.Categories;

public class GetCategoriesResponseDTO
{
    public required List<CategoryDTO> Categories { get; set; }
    public required int TotalCount { get; set; }
}