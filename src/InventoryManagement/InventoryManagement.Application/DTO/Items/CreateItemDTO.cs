namespace InventoryManagement.Application.DTO.Items;

public class CreateItemDTO
{
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required Guid CategoryId { get; set; }
}