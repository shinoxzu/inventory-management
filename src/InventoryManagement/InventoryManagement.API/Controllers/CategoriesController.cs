using InventoryManagement.API.Requests.ItemCategories;
using InventoryManagement.Application.DTO.Categories;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class CategoriesController(ICategoriesService categoriesService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var createCategoryDto = new CreateCategoryDTO
        {
            Name = request.Name,
            ParentId = request.ParentId
        };

        var newCategory = await categoriesService.CreateCategory(userId, createCategoryDto);
        var newCategoryUrl = new Uri(Request.GetEncodedUrl() + "/" + newCategory.Id);

        return Created(newCategoryUrl, newCategory);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<ActionResult> Update(Guid categoryId, [FromBody] CreateCategoryRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var createCategoryDto = new CreateCategoryDTO
        {
            Name = request.Name,
            ParentId = request.ParentId
        };
        await categoriesService.UpdateCategory(userId, categoryId, createCategoryDto);

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDTO>> Get(Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var category = await categoriesService.GetCategory(userId, id);
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<GetCategoriesResponseDTO>> GetMy([FromQuery] GetCategoriesRequest request)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        var getCategoriesDto = new GetCategoriesDTO
        {
            ParentId = request.ParentId
        };
        var categories = await categoriesService.GetUserCategories(userId, getCategoriesDto);
        return Ok(categories);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Remove(Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
        await categoriesService.RemoveCategory(userId, id);
        return Ok();
    }
}