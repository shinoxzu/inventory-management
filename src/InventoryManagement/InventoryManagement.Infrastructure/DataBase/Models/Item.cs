namespace InventoryManagement.Infrastructure.DataBase.Models;

public class Item
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required Guid AuthorId { get; set; }
    public required Guid CategoryId { get; set; }
}