namespace InventoryManagement.Application.DTO.Items;

public class ItemDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required Guid AuthorId { get; set; }
    public required Guid CategoryId { get; set; }
}