using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.OrderDTO;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService service;
        private readonly ICacheService cacheService;

        public OrderController(IOrderService service , ICacheService cacheService)
        {
            this.service = service;
            this.cacheService = cacheService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Check Order API health",
            Description = "This endpoint is used to test if the Order API is working correctly."
        )]
        [SwaggerResponse(200, "API is working successfully")]

        public IActionResult  Index()
        {
            return Ok("API is running successfully");
        }

        [HttpPost("Create")]
        [SwaggerOperation(
            Summary = "Create a new order",
            Description = "This endpoint allows you to create a new order with full details."
        )]
        [SwaggerResponse(201, "Order created successfully")]
        [SwaggerResponse(400, "Invalid order data")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize]

        public async Task<IActionResult> CreateOrder([FromBody] CreateOrUpdateOrder create)
        {
            if (create == null)
                return BadRequest("Order data cannot be null.");

            try
            {
                var result = await service.CreateOrder(create);
                await cacheService.RemoveAsync("all_orders");
                if (!string.IsNullOrWhiteSpace(create.CustomerEmail))
                    await cacheService.RemoveAsync($"orders_customer_{create.CustomerEmail}");
                if (!string.IsNullOrWhiteSpace(create.MerchantEmail))
                    await cacheService.RemoveAsync($"orders_merchant_{create.MerchantEmail}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Delete an existing order",
            Description = "This endpoint deletes a specific order by its ID."
        )]
        [SwaggerResponse(200, "Order deleted successfully")]
        [SwaggerResponse(400, "Invalid order ID")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Order ID.");

            try
            {
                var deleted = await service.DeleteOrder(id);
                if (!deleted)
                    return NotFound($"Order with ID {id} not found.");
                await cacheService.RemoveAsync("all_orders");
                await cacheService.RemoveAsync($"order_{id}");
                return Ok("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("GetById/{id:int}")]
        [SwaggerOperation(
            Summary = "Get order details",
            Description = "This endpoint retrieves a specific order's details using its ID."
        )]
        [SwaggerResponse(200, "Order retrieved successfully")]
        [SwaggerResponse(400, "Invalid order ID")]
        [SwaggerResponse(404, "Order not found")]
        [Authorize(Roles = "Admin,Merchant")]

        public async Task<IActionResult> GetOrderById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Order ID."); 
            
            string cacheKey = $"order_{id}";
            var cached = await cacheService.GetAsync<OrderDetailsDTO>(cacheKey);
            if (cached != null) return Ok(cached);

            try
            {
                var result = await service.GetOrderByIdAsync(id);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetAllOrders")]
        [SwaggerOperation(
            Summary = "Get all orders",
            Description = "This endpoint retrieves all orders from the database."

        )]
        [SwaggerResponse(200, "Orders retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllOrdersAsync()
        {
            string cacheKey = "all_orders";
            var cached = await cacheService.GetAsync<List<OrderListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);

            try
            {
                var result = await service.GetAllOrdersAsync();
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetCustomerOrderCountAsync/{email}")]
        [Authorize(Roles = "Admin,Customer")]

        [SwaggerOperation(
            Summary = "Get number of customer orders",
            Description = "This endpoint retrieves the total number of orders placed by a specific customer using their email."
        )]
        [SwaggerResponse(200, "Order count retrieved successfully")]
        [SwaggerResponse(400, "Invalid email")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetCustomerOrderCountAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email cannot be empty.");
            string cacheKey = $"orders_count_{email}";
            var cached = await cacheService.GetAsync<int?>(cacheKey);
            if (cached.HasValue) return Ok(new { Email = email, OrdersCount = cached.Value });
            try
            {
                var result = await service.GetCustomerOrderCountAsync(email);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(new { Email = email, OrdersCount = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetOrdersByCustomerEmailAsync/{CustomerEmail}")]
        [Authorize(Roles = "Admin,Customer")]

        [SwaggerOperation(
            Summary = "Get customer orders",
            Description = "This endpoint retrieves all orders for a specific customer using their email."
        )]
        [SwaggerResponse(200, "Orders retrieved successfully")]
        [SwaggerResponse(400, "Invalid email")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetOrdersByCustomerEmailAsync(string CustomerEmail)
        {
            if (string.IsNullOrEmpty(CustomerEmail))
                return BadRequest("Email cannot be empty.");
            string cacheKey = $"orders_customer_{CustomerEmail}";
            var cached = await cacheService.GetAsync<List<OrderByCustomerDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var result = await service.GetOrdersByCustomerEmailAsync(CustomerEmail);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetOrdersByMerchantEmail/{email}")]
        [Authorize(Roles = "Admin,Merchant")]

        [SwaggerOperation(
            Summary = "Get merchant orders",
            Description = "This endpoint retrieves all orders received by a specific merchant using their email."
        )]
        [SwaggerResponse(200, "Orders retrieved successfully")]
        [SwaggerResponse(400, "Invalid email")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetOrdersByMerchantEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email cannot be empty.");
            string cacheKey = $"orders_merchant_{email}";
            var cached = await cacheService.GetAsync<List<OrderListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var result = await service.GetOrdersByMerchantEmailAsync(email);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Admin,Merchant")]
        [HttpGet("SearchOrders")]
        [SwaggerOperation(
            Summary = "Search orders",
            Description = "This endpoint allows searching for orders using multiple filters such as customer, merchant, or status."
        )]
        [SwaggerResponse(200, "Orders retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> SearchOrdersAsync([FromQuery] SearchOrderDTO search)
        {
            try
            {
                var result = await service.SearchOrdersAsync(search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin,Customer")]

        [HttpPut("Update/{id:int}")]
        [SwaggerOperation(Summary = "Update an existing order", Description = "Updates an existing order using its ID and returns the updated details.")]
        [SwaggerResponse(200, "Order updated successfully")]
        [SwaggerResponse(400, "Invalid order data")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrUpdateOrder update)
        {
            if (id <= 0)
                return BadRequest("Invalid Order ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await service.UpdateOrder(id, update);
                await cacheService.RemoveAsync("all_orders");
                await cacheService.RemoveAsync($"order_{id}");
                if (!string.IsNullOrWhiteSpace(update.CustomerEmail))
                    await cacheService.RemoveAsync($"orders_customer_{update.CustomerEmail}");
                if (!string.IsNullOrWhiteSpace(update.MerchantEmail))
                    await cacheService.RemoveAsync($"orders_merchant_{update.MerchantEmail}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
