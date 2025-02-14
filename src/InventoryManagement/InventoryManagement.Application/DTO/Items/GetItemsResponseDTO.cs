namespace InventoryManagement.Application.DTO.Items;

public class GetItemsResponseDTO
{
    public required List<ItemDTO> Items { get; set; }
    public required int TotalCount { get; set; }
}