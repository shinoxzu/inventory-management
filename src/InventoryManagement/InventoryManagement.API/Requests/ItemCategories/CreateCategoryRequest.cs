namespace InventoryManagement.API.Requests.ItemCategories;

public record CreateCategoryRequest(string Name, Guid? ParentId);