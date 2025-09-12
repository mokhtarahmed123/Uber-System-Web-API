using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.ComplaintsDTOs;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintsService complaintsService;
        private readonly ICacheService cacheService;

        public ComplaintController(IComplaintsService complaintsService, ICacheService cacheService)
        {
            this.complaintsService = complaintsService;
            this.cacheService = cacheService;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Check if Complaint API is working")]
        [SwaggerResponse(200, "Complaint API is working.")]
        public IActionResult Index()
        {
            return Ok("Complaint API is working.");
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Customer,Merchant,Driver")]
        [SwaggerOperation(
            Summary = "Create a new complaint",
            Description = "This endpoint allows you to create a new complaint and store it in the database."
        )]
        [SwaggerResponse(200, "Complaint created successfully")]
        [SwaggerResponse(400, "Invalid request body")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Create([FromBody] CreateComplaintsdto Create)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            try
            {

                var Result = await complaintsService.CreateAsync(Create);
                await cacheService.RemoveAsync("all_complaints");

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }

        }

        [HttpDelete("Delete/{id:int} ")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
        Summary = "Delete a complaint by ID",
        Description = "Deletes a specific complaint using its ID.")]
        [SwaggerResponse(200, "Complaint deleted successfully")]
        [SwaggerResponse(400, "Invalid ID")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "Unexpected error occurred")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id Must Be Greater Than 0 ");
            try
            {
                var Result = await complaintsService.DeleteAsync(id);
                await cacheService.RemoveAsync("all_complaints");
                await cacheService.RemoveAsync($"complaint_{id}");
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetcomplaintById/{id:int}")]
        [SwaggerOperation(Summary = "Get complaint by ID", Description = "Retrieve a complaint using its ID.")]
        [SwaggerResponse(200, "Complaint retrieved successfully", typeof(CreateComplaintsdto))]
        [SwaggerResponse(400, "Invalid ID")]
        [SwaggerResponse(404, "Complaint not found")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Id Must Be Greater Than 0 ");
            string cacheKey = $"complaint_{id}";
            var cached = await cacheService.GetAsync<CreateComplaintsdto>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            try
            {
                var result = await complaintsService.GetComplaintByIdAsync(id);
                if (result == null) return NotFound($"Complaint {id} not found");

                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("GetAll Complaints")]
        [Authorize(Roles = "Admin")]

        [SwaggerOperation(Summary = "Get all complaints", Description = "Retrieve a list of all complaints.")]
        [SwaggerResponse(200, "List of complaints returned successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "all_complaints";
            var cached = await cacheService.GetAsync<List<CreateComplaintsdto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }
            try
            {
                var Result = await complaintsService.GetAllComplaintsAsync();
                await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(5)); 


                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Resolve/{id:int}")]
        [SwaggerOperation(
            Summary = "Resolve a complaint",
            Description = "Mark a complaint as resolved using its ID."
        )]
        [SwaggerResponse(200, "Complaint resolved successfully")]
        [SwaggerResponse(400, "Invalid ID or data")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> ResolveComplaintAsync(int id, [FromBody] ResolveComplaintDTO resolve)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");
            string cacheKey = $"complaint_{id}";
            var cached = await cacheService.GetAsync<CreateComplaintsdto>(cacheKey);
            if (cached != null)
                return Ok(cached);



            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await complaintsService.ResolveComplaintAsync(id, resolve);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("Search")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Search complaints",
            Description = "Search complaints using filters like status, date, and keywords."
        )]
        [SwaggerResponse(200, "Search completed successfully")]
        [SwaggerResponse(400, "Invalid search parameters")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> SearchComplaintsAsync([FromQuery] SearchComplaintDTO search)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await complaintsService.SearchComplaintsAsync(search);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");

            }

        }
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Update a complaint",
            Description = "Update an existing complaint using its ID."
        )]
        [SwaggerResponse(200, "Complaint updated successfully")]
        [SwaggerResponse(400, "Invalid ID or data")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> Update([FromBody] UpdateComplaintsdto update, int id)
        {
            if (id <= 0)
                return BadRequest("Id Must Be Greater Than 0 ");
            string cacheKey = $"complaint_{id}";
            var cached = await cacheService.GetAsync<CreateComplaintsdto>(cacheKey);
            if (cached != null)
                return Ok(cached);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                update.IsResolved = false;
                var Result = await complaintsService.UpdateAsync(update, id);
                await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetComplaintsByCustomer")]
        [Authorize]
        [SwaggerOperation(Summary = "Get All  Complaints For Authorize User ")]

        public async Task<IActionResult> GetComplaintByCustomer()
        {
            try
            {
                var Result = await complaintsService.GetComplaintByCustomer();

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }


        }





    }
}
