namespace InventoryManagement.API.Requests.Items;

public record CreateItemRequest(string Name, int Count, Guid CategoryId);