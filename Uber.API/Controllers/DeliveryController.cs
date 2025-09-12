    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
    using Uber.Uber.Application.DTOs.DeliveryDTOs;
    using Uber.Uber.Application.Interfaces;
    using Uber.Uber.Application.Services;
    using Uber.Uber.Domain.Entities.Enums;

    namespace Uber.Uber.API.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class DeliveryController : ControllerBase
        {
            private readonly IDeliveryService deliveryService;
            private readonly ICacheService cacheService;
        private readonly IHubContext<DeliveryHub> deliveryHub;

        public DeliveryController(IDeliveryService deliveryService, ICacheService cacheService, IHubContext<DeliveryHub> deliveryHub)
            {
                this.deliveryService = deliveryService;
                this.cacheService = cacheService;
            this.deliveryHub = deliveryHub;
        }

            [HttpGet]
            [SwaggerOperation(Summary = "Check if Delivery API is working")]
            [SwaggerResponse(200, "Delivery API is working.")]
            public IActionResult Index()
            {
                return Ok("Delivery API is working.");

            }

            [HttpPost("CreateDelivery")]
            [Authorize(Roles = "Admin")]

            [SwaggerOperation(
                Summary = "Create a delivery without an order",
                Description = "This endpoint creates a delivery that is not linked to an order."
            )]
            [SwaggerResponse(200, "Delivery created successfully")]
            [SwaggerResponse(400, "Invalid request body")]
            [SwaggerResponse(500, "Unexpected server error")]
            public async Task<IActionResult> CreateDelivery([FromBody] CreateDeliveryDTO deliveryDTO)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                try
                {
                    var Result = await deliveryService.CreateDeliveryDTO(deliveryDTO);
                    await cacheService.RemoveAsync("all_deliveries");
                await deliveryHub.Clients.Group("Admins")
                   .SendAsync("DeliveryUpdate", Result.TripId , Result.Status);
                return Ok(Result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }

            }

       

            [HttpGet("GetDeliveriesByStatus")]
            [Authorize(Roles = "Admin,Driver")]

            [SwaggerOperation(
                Summary = "Get deliveries by status",
                Description = "Retrieve all deliveries filtered by their current status."
            )]
            [SwaggerResponse(200, "List of deliveries by status returned successfully")]
            [SwaggerResponse(400, "Invalid status provided")]
            [SwaggerResponse(500, "Unexpected server error")]
            public async Task<IActionResult> GetDeliveriesByStatusAsync([FromQuery] DeliveryStatus status)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                string cacheKey = $"deliveries_status_{status}";

                var cached = await cacheService.GetAsync<List<CreateDeliveryDTO>>(cacheKey);
                if (cached != null)
                    return Ok(cached);
                try
                {
                    var Result = await deliveryService.GetDeliveriesByStatusAsync(status);
                    await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(5));
                    return Ok(Result);

                }
                catch (Exception ex)
                {

                    return StatusCode(500, $"Error: {ex.Message}");

                }
            }

            [HttpGet("GetDeliveryByIdAsync/{id:int}")]
            [SwaggerOperation(Summary = "Get a delivery by ID", Description = "Retrieve a specific delivery using its ID.")]
            [SwaggerResponse(200, "Delivery retrieved successfully", typeof(CreateDeliveryDTO))]
            [SwaggerResponse(400, "Invalid ID")]
            [SwaggerResponse(404, "Delivery not found")]
            [SwaggerResponse(500, "Unexpected server error")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> GetDeliveryByIdAsync(int id)
            {
                if (id <= 0)
                    return BadRequest("Id Must Be GREATER THAN 0");
                string cacheKey = $"delivery_{id}";

                var cached = await cacheService.GetAsync<CreateDeliveryDTO>(cacheKey);
                if (cached != null)
                    return Ok(cached);
                try
                {
                    var result = await deliveryService.GetDeliveryByIdAsync(id);
                    if (result == null)
                        return NotFound($"Delivery with ID {id} not found.");

                    await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }
            }

            [HttpGet("ListDeliveries")]
            [SwaggerOperation(Summary = "Get all deliveries", Description = "Retrieve a list of all deliveries.")]
            [SwaggerResponse(200, "List of deliveries retrieved successfully")]
            [SwaggerResponse(500, "Unexpected server error")]
            [Authorize(Roles = "Admin")]

            public async Task<IActionResult> ListDelivery()
            {
                string cacheKey = "all_deliveries";
                var cached = await cacheService.GetAsync<List<CreateDeliveryDTO>>(cacheKey);
                if (cached != null)
                    return Ok(cached);
                try
                {
                    var Result = await deliveryService.ListDeliveryDTO();
                    await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                    return Ok(Result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");

                }

            }

            [HttpGet("SearchDeliveriesAsync")]
            [Authorize(Roles = "Admin")]

            [SwaggerOperation(
                Summary = "Search deliveries",
                Description = "Search deliveries using filters like status, driver, and date range."
            )]
            [SwaggerResponse(200, "Deliveries found successfully")]
            [SwaggerResponse(400, "Invalid search parameters")]
            [SwaggerResponse(500, "Unexpected server error")]
            public async Task<IActionResult> SearchDeliveriesAsync([FromQuery] SearchDeliveryDTO searchDTO)
            {
                string cacheKey = $"deliveries_search_{searchDTO.DriverEmail}";
                var cached = await cacheService.GetAsync<List<CreateDeliveryDTO>>(cacheKey);
                if (cached != null)
                    return Ok(cached);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var Result = await deliveryService.SearchDeliveriesAsync(searchDTO);
                    await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(5));

                    return Ok(Result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }

            }

            [HttpPut("UpdateDelivery/{id:int}")]
            [SwaggerOperation(
                Summary = "Update delivery details",
                Description = "Update the details of an existing delivery without changing the order."
            )]
            [SwaggerResponse(200, "Delivery updated successfully")]
            [SwaggerResponse(400, "Invalid ID or request body")]
            [SwaggerResponse(404, "Delivery not found")]
            [SwaggerResponse(500, "Unexpected server error")]
            [Authorize(Roles = "Admin,Driver")]
            public async Task<IActionResult> UpdateDeliveryDTO(int id, UpdateDeliveryDTO deliveryDTO)
            {
                if (id <= 0) return BadRequest("Id Must Be Greater Than 0");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                try
                {
                    var Result = await deliveryService.UpdateDeliveryDTO(id, deliveryDTO);
                    await cacheService.RemoveAsync($"delivery_{id}");
                    await cacheService.RemoveAsync("all_deliveries");
                await deliveryHub.Clients.Group("Admins")
           .SendAsync("DeliveryUpdate", id, Result.Status.ToString());
                return Ok(Result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }
            }    
        




        }
    }