namespace InventoryManagement.Infrastructure.DataBase.Models;

public class Category
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid? ParentId { get; set; }
    public required Guid AuthorId { get; set; }
}