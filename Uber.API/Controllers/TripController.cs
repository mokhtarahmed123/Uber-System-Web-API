using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.TripDTOs;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Application.Services;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService tripService;
        private readonly ICacheService _cacheService;
        private readonly IHubContext<TripHub> tripHub;

        public TripController(ITripService tripService , ICacheService cacheService, IHubContext<TripHub> tripHub)
        {
            this.tripService = tripService;
            this._cacheService = cacheService;
            this.tripHub = tripHub;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "API Status Check", Description = "Verify if the Trip API is working.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip API is up and running.")]
        public IActionResult Index()
        {
            return Ok("Trip API is working.");
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("CreateTrip")]
        [SwaggerOperation(Summary = "Create a new trip", Description = "Creates a new trip based on the provided details.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid trip data provided.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Create([FromBody] CreateTripDTO create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var Result = await tripService.CreateTripAsync(create);
                var tripReferenceId = create.OrderId ?? create.RideRequestId ?? 0;


                await _cacheService.RemoveAsync("AllTrips");

                await tripHub.Clients.User(create.DriverEmail)
                    .SendAsync("ReceiveNewTrip", tripReferenceId);
                return Ok(Result);

            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteTrip/{id:int}")]
        [SwaggerOperation(Summary = "Delete a trip", Description = "Deletes a trip by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid trip ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id Must Be Greater Than 0!!!!!!!!!!!!!! ");
            }
            try
            {
                var Result = await tripService.DeleteTripAsync(id);
                await _cacheService.RemoveAsync("AllTrips");
                await _cacheService.RemoveAsync($"Trip_{id}");
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("GetTripsbydriverEmail/{Email}")]
        [SwaggerOperation(Summary = "Get trips by driver email", Description = "Retrieves all trips assigned to a specific driver.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trips retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Driver email cannot be null.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetTripdriverEmail(string Email)
        {
            if (Email == null)
            {
                return BadRequest("Email Can't Be Null ");
            }
            string cacheKey = $"TripsByDriver_{Email}";
            var cached = await _cacheService.GetAsync<List<TripListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await tripService.GetTripsByDriverAsync(Email);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllTrips")]
        [SwaggerOperation(Summary = "Get all trips", Description = "Fetches a list of all available trips.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trips retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "AllTrips";
            var cached = await _cacheService.GetAsync<List<TripListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);

            try
            {
                var Result = await tripService.GetAllTripsAsync();
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }
        [HttpGet("GetTripDetailsAsync/{id:int}")]
        [Authorize(Roles = "Admin,Driver")]
        [SwaggerOperation(Summary = "Get trip details", Description = "Fetches detailed information for a specific trip by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip details retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid trip ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetTripDetailsAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id Must Be Greater Than 0!!!!!!!!!!!!!! ");
            }
            string cacheKey = $"Trip_{id}";
            var cached = await _cacheService.GetAsync<TripDetailsDTO>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await tripService.GetTripDetailsAsync(id);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);

            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("SearchTrip")]
        [SwaggerOperation(Summary = "Search trips", Description = "Searches trips using filtering parameters.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trips retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid search criteria.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> SearchTrip([FromQuery] SearchTripDTO searchTripDTO)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            string cacheKey = $"SearchTrip_{searchTripDTO.DriverEmail}_{searchTripDTO.RiderEmail}_{searchTripDTO.StatausTrip}";
            var cached = await _cacheService.GetAsync<List<SearchTripDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await tripService.SearchTripDTO(searchTripDTO);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }

        [HttpPut("UpdateTripAsync/{id:int}")]
        [SwaggerOperation(Summary = "Update a trip", Description = "Updates the details of an existing trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> UpdateTripAsync(int id, UpdateTripDTO update)
        {
            if (id <= 0)
            {
                return BadRequest("Id Must Be Greater Than 0!!!!!!!!!!!!!! ");
            }
            if (!ModelState.IsValid) { 
                    return BadRequest(ModelState);
            }
            try
            {
                var Result  = await tripService.UpdateTripAsync(id, update);
                await _cacheService.RemoveAsync("AllTrips");
                await _cacheService.RemoveAsync($"Trip_{id}");
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }


        }

        [HttpPut("UpdateTripStatusAsync/{id:int}")]
        [SwaggerOperation(Summary = "Update trip status", Description = "Updates the status of an existing trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip status updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid status update request.")]
        [Authorize(Roles = "Admin,Driver")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> UpdateTripStatusAsync(int id, UpdateTripStatusDTO update)
        {
            if (id <= 0)
            {
                return BadRequest("Id Must Be Greater Than 0!!!!!!!!!!!!!! ");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var Result = await tripService.UpdateTripStatusAsync(id, update);
                await _cacheService.RemoveAsync($"Trip_{id}");
                await _cacheService.RemoveAsync("AllTrips");
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }
    }
 }
