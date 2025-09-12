using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application.DTOs.CategoryDTO;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly UberContext context;
        private readonly IConfiguration configuration;
        private readonly ICategoryService service;
        private readonly ICacheService cacheService;

        public CategoryController(UberContext context, IConfiguration configuration, ICategoryService service,ICacheService cacheService)
        {
            this.context = context;
            this.configuration = configuration;
            this.service = service;
            this.cacheService = cacheService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Check if Category API is working")]
        [SwaggerResponse(200, "Category API is working.")]
        public ActionResult Index()
        {
            return Ok("Category API is working.");
        }
        [SwaggerOperation(
            Summary = "Creates a new Category",
            Description = "This endpoint allows you to create a new category and store it in the database."
        )]
        [SwaggerResponse(201, "Category Added Successfully")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] CreateOrUpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await service.CreateCategory(dto);
                await cacheService.RemoveAsync("all_categories");
                return Created();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all categories", Description = "Retrieve all categories from the database.")]
        [SwaggerResponse(200, "List of categories returned successfully",type:typeof(List<CreateOrUpdateCategoryDTO>))]
        [AllowAnonymous]

        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "all_categories";
            var list = await cacheService.GetAsync<List<Category>>(cacheKey);

            if (list == null)
            {
                list = await service.GetAllCategories();

                await cacheService.SetAsync(cacheKey, list, TimeSpan.FromMinutes(5));
            }
            return Ok(list);
        }

        [SwaggerOperation(Summary = "Get category by ID", Description = "Retrieve a specific category by its ID.")]
        [SwaggerResponse(200, "Category found successfully", typeof(CreateOrUpdateCategoryDTO))]
        [SwaggerResponse(400, "ID must be greater than 0")]
        [SwaggerResponse(404, "Category not found")]
        [HttpGet("GetByID/{ID:int}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetByID(int ID)
        {
            string cacheKey = $"category_{ID}";
            var category = await cacheService.GetAsync<CreateOrUpdateCategoryDTO>(cacheKey);
            if (category == null)
            {
                category = await service.GetCategoryById(ID);

                if (category == null)
                    return NotFound($"Category with ID {ID} not found.");

                await cacheService.SetAsync(cacheKey, category, TimeSpan.FromMinutes(10));
            }

            return Ok(category);
        }

        [SwaggerOperation(Summary = "Get category with items", Description = "Retrieve a category and all its related items.")]
        [SwaggerResponse(200, "Category with items found", typeof(GetCategoryWithItems))]
        [SwaggerResponse(400, "ID must be greater than 0")]
        [SwaggerResponse(404, "Category or items not found")]
        [AllowAnonymous]
        [HttpGet("GetCategoryWithItems/{ID:int}")]
        public async Task<IActionResult> GetCategoryWithItems(int ID)
        {
            if (ID <= 0)
                return BadRequest("ID must be greater than 0");

            var category = await service.GetCategoryWithItems(ID);

            if (category == null)
                return NotFound($"Category with ID {ID} not found.");

            return Ok(category);

        }

        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update a category", Description = "Update an existing category using its ID.")]
        [SwaggerResponse(200, "Category updated successfully")]
        [SwaggerResponse(400, "Invalid data or ID")]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [HttpPut("UpdateCategory/{ID:int}")]
        public async Task<IActionResult> UpdateCategory(int ID, [FromBody] CreateOrUpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (ID <= 0)
                return BadRequest("ID must be greater than 0");
            try
            {
                await cacheService.RemoveAsync("all_categories");
                await cacheService.RemoveAsync($"category_{ID}");
                var createdCategory = await service.UpdateCategory(ID, dto);
                return Ok(createdCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a category", Description = "Delete a specific category using its ID.")]
        [SwaggerResponse(200, "Category deleted successfully")]
        [SwaggerResponse(400, "ID must be greater than 0")]
        [SwaggerResponse(404, "Category not found")]

        [HttpDelete("DeleteCategory/{ID:int}")]
        public async Task<IActionResult> DeleteCategory(int ID)
        {
            if (ID <= 0)
                return BadRequest("ID must be greater than 0");

            var deleted = await service.DeleteCategory(ID);

            if (!deleted) return NotFound($"Category with ID {ID} not found.");

            // 🧹 Clear cache
            await cacheService.RemoveAsync("all_categories");
            await cacheService.RemoveAsync($"category_{ID}");

            return Ok(deleted);
        }
    }
}
