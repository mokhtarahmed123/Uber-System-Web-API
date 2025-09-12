using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly UberContext context;
        private readonly IConfiguration configuration;
        private readonly IItemService service;
        private readonly ICacheService cacheService;

        public ItemController(UberContext context, IConfiguration configuration, IItemService service , ICacheService cacheService)
        {
            this.context = context;
            this.configuration = configuration;
            this.service = service;
            this.cacheService = cacheService;
        }
        [HttpGet]
        [SwaggerResponse(200, "API is working successfully.")]

        public ActionResult Index()
        {
            return Ok();
        }

        [HttpPost("AddItem")]
        [SwaggerResponse(201, "Item created successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        [Authorize(Roles = "Admin,Merchant")]


        public async Task<IActionResult> AddItem([FromBody] CreateandUpdateItemDTO itemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await service.CreateItemAsync(itemDTO);
                await cacheService.RemoveAsync("all_items");
                if (!string.IsNullOrWhiteSpace(itemDTO.CategoryName))
                    await cacheService.RemoveAsync($"items_category_{itemDTO.CategoryName}");
                if (!string.IsNullOrWhiteSpace(itemDTO.MerchantEmail))
                    await cacheService.RemoveAsync($"items_merchant_{itemDTO.MerchantEmail}");
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
        [AllowAnonymous]

        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "all_items";
            var cached = await cacheService.GetAsync<List<ItemListDTO>>(cacheKey);
            if (cached != null)
                return Ok(cached);

            var GEtALL = await service.GetAllItems();
            await cacheService.SetAsync(cacheKey, GEtALL, TimeSpan.FromMinutes(10));
            return Ok(GEtALL);

        }

        [HttpGet("GetItemById/{id:int}")]
        [SwaggerResponse(200, "Item retrieved successfully.")]
        [SwaggerResponse(400, "Invalid ID.")]
        [SwaggerResponse(404, "Item not found.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        [AllowAnonymous]

        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
                return BadRequest(" Id Must By Greater Than 0 ");
            string cacheKey = $"item_{id}";
            var cached = await cacheService.GetAsync<GetItemDTO>(cacheKey);
            if (cached != null)
                return Ok(cached);
            var item = await service.GetItemById(id);
            if (item == null)
                return NotFound($"Item with ID {id} not found.");
            await cacheService.SetAsync(cacheKey, item, TimeSpan.FromMinutes(10));

            return Ok(item);
        }

        [HttpGet("GetItemByCategoryName/{Name:alpha}")]
        [SwaggerResponse(200, "Items retrieved successfully.")]
        [SwaggerResponse(400, "Category name is required.")]
        [SwaggerResponse(404, "Category not found.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        [AllowAnonymous]

        public async Task<IActionResult> GetItemByCategoryName(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return BadRequest("Category name is required.");

            string cacheKey = $"items_category_{Name}";
            var cached = await cacheService.GetAsync<List<ItemListDTO>>(cacheKey);
            if (cached != null)
                return Ok(cached);
            if (Name == null)
                return BadRequest(" Please Enter Name ");
            var item = await service.GetItemsByCategory(Name);
            if (item == null)
                return NotFound($"Category  with Name {Name} not found.");
            await cacheService.SetAsync(cacheKey, item, TimeSpan.FromMinutes(10));

            return Ok(item);
        }
        [HttpGet("GetItemByMerchantEmail/{Email}")]
        [SwaggerResponse(200, "Items retrieved successfully.")]
        [SwaggerResponse(400, "Merchant email is required.")]
        [SwaggerResponse(404, "Merchant not found.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        [Authorize]

        public async Task<IActionResult> GetItemByMerchantEmail(string Email)
        {
            if (Email == null)
                return BadRequest(" Please Enter Email ");
            if (string.IsNullOrWhiteSpace(Email))
                return BadRequest("Merchant email is required.");

            string cacheKey = $"items_merchant_{Email}";
            var cached = await cacheService.GetAsync<List<ItemListDTO>>(cacheKey);
            if (cached != null)
                return Ok(cached);
            var item = await service.GetItemsByMerchant(Email);
            if (item == null)
                return NotFound($"Merchant  with Email {Email} not found.");
            await cacheService.SetAsync(cacheKey, item, TimeSpan.FromMinutes(10));
            return Ok(item);
        }
        [HttpPut("UpdateItem/{id:int}")]
        [SwaggerResponse(200, "Item updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Item not found.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        [Authorize(Roles = "Admin,Merchant")]

        public async Task<IActionResult> UpdateItem(int id, CreateandUpdateItemDTO itemDTO)
        {
            if (id <= 0)
                return BadRequest(" Id Must By Greater Than 0 ");


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await service.UpdateItemAsync(id, itemDTO);
                await cacheService.RemoveAsync("all_items");
                await cacheService.RemoveAsync($"item_{id}");
                if (!string.IsNullOrWhiteSpace(itemDTO.CategoryName))
                    await cacheService.RemoveAsync($"items_category_{itemDTO.CategoryName}");
                if (!string.IsNullOrWhiteSpace(itemDTO.MerchantEmail))
                    await cacheService.RemoveAsync($"items_merchant_{itemDTO.MerchantEmail}");
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
        [Authorize(Roles = "Admin,Merchant")]

        [HttpDelete("DeleteItem/{id:int}")]
        [SwaggerResponse(200, "Item deleted successfully.")]
        [SwaggerResponse(400, "Invalid ID.")]
        [SwaggerResponse(404, "Item not found.")]
        [SwaggerResponse(500, "Unexpected server error.")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0)
                return BadRequest(" Id Must By Greater Than 0 ");
            var Item = await service.DeleteItem(id);
            await cacheService.RemoveAsync("all_items");
            await cacheService.RemoveAsync($"item_{id}");
            return Ok(Item);

        }






    }
}
