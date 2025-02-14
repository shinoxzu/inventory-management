using InventoryManagement.API.Requests.Items;
using InventoryManagement.Application.DTO.Items;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class ItemsController(IItemsService itemsService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ItemDTO>> Create([FromBody] CreateItemRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var createItemDto = new CreateItemDTO
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Count = request.Count
        };

        var newItem = await itemsService.CreateItem(userId, createItemDto);
        var newItemUrl = new Uri(Request.GetEncodedUrl() + "/" + newItem.Id);

        return Created(newItemUrl, newItem);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<ActionResult> Update(Guid categoryId, [FromBody] CreateItemRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var createItemDto = new CreateItemDTO
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            Count = request.Count
        };
        await itemsService.UpdateItem(userId, categoryId, createItemDto);

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDTO>> Get(Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var item = await itemsService.GetItem(userId, id);
        return Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<GetItemsResponseDTO>> GetMy([FromQuery] GetItemsRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var getItemsDto = new GetItemsDTO
        {
            CategoryId = request.CategoryId
        };
        var items = await itemsService.GetUserItems(userId, getItemsDto);
        return Ok(items);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Remove(Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        await itemsService.RemoveItem(userId, id);
        return Ok();
    }
}