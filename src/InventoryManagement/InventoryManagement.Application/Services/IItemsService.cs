using InventoryManagement.Application.DTO.Items;

namespace InventoryManagement.Application.Services;

public interface IItemsService
{
    Task<ItemDTO> GetItem(Guid fromUserId, Guid itemId);

    Task<GetItemsResponseDTO> GetUserItems(Guid fromUserId, GetItemsDTO getItemsDto);

    Task<ItemDTO> CreateItem(Guid fromUserId, CreateItemDTO createItemDto);

    Task RemoveItem(Guid fromUserId, Guid itemId);

    Task UpdateItem(Guid fromUserId, Guid itemId, CreateItemDTO createItemDto);
}