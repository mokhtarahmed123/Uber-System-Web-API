using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs;
using Uber.Uber.Application.DTOs.DriverProfileDTOS;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverProfileController : ControllerBase
    {
        private readonly IDriverProfileService profileService;
        private readonly ILogger<DriverProfileController> logger;
        private readonly ICacheService cacheService;
        private readonly IHubContext<DriverHub> driverHub;

        public DriverProfileController(IDriverProfileService profileService, ILogger<DriverProfileController> logger , ICacheService cacheService ,     IHubContext<DriverHub> driverHub
)
        {
            this.profileService = profileService;
            this.logger = logger;
            this.cacheService = cacheService;
            this.driverHub = driverHub;
        }

        [HttpGet]
        
        [SwaggerOperation(Summary = "Check Driver Profile API health", Description = "Simple check endpoint.")]
        [SwaggerResponse(200, "Driver Profile API is working.")]
        public IActionResult Index()
        {
            return Ok("Driver Profile API is working.");
        }

        [HttpPost("CreateDriverProfile")]
        [SwaggerOperation(Summary = "Create a new driver profile", Description = "Creates a new driver profile using provided data.")]
        [SwaggerResponse(201, "Driver profile created successfully")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]


        public async Task<IActionResult> Create([FromBody] CreateDriverProfile createDriverProfile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await profileService.CreateAsync(createDriverProfile);
                await cacheService.RemoveAsync("all_driver_profiles");
                await cacheService.RemoveAsync($"driver_email_{createDriverProfile.DriverEmail}");
                await driverHub.Clients.All.SendAsync("DriverProfileCreated", result);

                return CreatedAtAction(nameof(GetDetailsByEmailAsync), new { Email = createDriverProfile.DriverEmail }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating driver profile");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("DeleteDriverProfile/{id:int}")]
        [SwaggerOperation(Summary = "Delete driver profile", Description = "Deletes a driver profile by its ID.")]
        [SwaggerResponse(200, "Driver profile deleted successfully")]
        [SwaggerResponse(400, "Invalid ID")]
        [SwaggerResponse(404, "Driver profile not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await profileService.DeleteAsync(id);
                await cacheService.RemoveAsync("all_driver_profiles");
                await cacheService.RemoveAsync($"driver_id_{id}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting driver profile with id: {id}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all driver profiles", Description = "Fetches all driver profiles.")]
        [SwaggerResponse(200, "Driver profiles fetched successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllAsync()
        {
            string cacheKey = "all_driver_profiles";

            var cached = await cacheService.GetAsync<List<GetAllDriversDTO>>(cacheKey);
            if (cached != null)
                return Ok(cached);
            try
            {
                var result = await profileService.GetAllAsync();
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching driver profiles");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpGet("GetDetailsByEmail/{Email}")]
        [SwaggerOperation(Summary = "Get driver profile by email", Description = "Fetches driver profile details using email.")]
        [SwaggerResponse(200, "Driver profile fetched successfully")]
        [SwaggerResponse(400, "Invalid email provided")]
        [SwaggerResponse(404, "Driver profile not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> GetDetailsByEmailAsync(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
                return BadRequest("Email can't be null or empty.");

            string cacheKey = $"driver_email_{Email}";

            var cached = await cacheService.GetAsync<GetDriverProfilesDetails>(cacheKey);
            if (cached != null)
                return Ok(cached);
            try
            {
                var result = await profileService.GetDetailsByEmailAsync(Email);
                if (result == null)
                    return NotFound($"Driver profile not found for email {Email}");

                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error fetching driver profile by email: {Email}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPut("ChangeStatus/{id:int}")]
        [SwaggerOperation(Summary = "Change driver status", Description = "Updates driver profile status by ID.")]
        [SwaggerResponse(200, "Driver status updated successfully")]
        [SwaggerResponse(400, "Invalid ID or data provided")]
        [SwaggerResponse(404, "Driver profile not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> ChangeStatusAsync(int id, [FromBody] ChangeDriverStatusDTO changeDriverStatusDTO)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await profileService.ChangeStatusAsync(id, changeDriverStatusDTO);

                await cacheService.RemoveAsync("all_driver_profiles");
                await cacheService.RemoveAsync($"driver_id_{id}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error changing driver status for id: {id}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPut("Update/{id:int}")]
        [SwaggerOperation(Summary = "Update driver profile", Description = "Updates driver profile information by ID.")]
        [SwaggerResponse(200, "Driver profile updated successfully")]
        [SwaggerResponse(400, "Invalid ID or data provided")]
        [SwaggerResponse(404, "Driver profile not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateDriverProfile updateDriverProfile)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await profileService.UpdateAsync(id, updateDriverProfile);

                await cacheService.RemoveAsync("all_driver_profiles");
                await cacheService.RemoveAsync($"driver_id_{id}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating driver profile for id: {id}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}

