using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.RideRequestDTOs;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Application.Services;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RideRequestsController : ControllerBase
    {
        private readonly IRideRequestsService requestsService;
        private readonly ICacheService _cacheService;

        public RideRequestsController(IRideRequestsService requestsService , ICacheService  cacheService)
        {
            this.requestsService = requestsService;
            this._cacheService = cacheService;
        }

        //[AllowAnonymous]

        [HttpGet]
        [SwaggerOperation(Summary = "API Status Check", Description = "Verify if the Ride Request API is working.")]
        [SwaggerResponse(StatusCodes.Status200OK, "RideR equest API is up and running.")]
        public IActionResult Index()
        {
            return Ok("Ride Request API is working.");
        }


        [HttpPost("Create")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Create a new Ride Request ", Description = "Creates a new Ride Request based on the provided details.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ride Request created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request data provided.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Create([FromBody] CreateRideRequestDTO create)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var riderEmail = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(riderEmail))
                    return Unauthorized("Invalid Rider. Please login again.");

                create.RiderEmail = riderEmail;

                var result = await requestsService.CreateRideRequest(create);
                await _cacheService.RemoveAsync("AllRideRequests");
                await _cacheService.RemoveAsync($"PendingRequests_{riderEmail}");
                return Ok(new { Status = "Success", Message = "Ride Request created successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRideRequest/{id:int}")]
        [SwaggerOperation(Summary = "Delete a Ride Request", Description = "Deletes a Ride Request by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await requestsService.DeleteRequest(id);
                await _cacheService.RemoveAsync("AllRideRequests");
                await _cacheService.RemoveAsync($"AcceptedRequests");
                await _cacheService.RemoveAsync($"CompletedRequests");
                return Ok(new { Status = "Success", Message = "Ride Request deleted successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPut("AcceptRideRequest/{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Accept a Ride Request", Description = "Accept a Ride Request by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip Accepted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> AcceptRideRequest(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await requestsService.AcceptRideRequest(id);
                await _cacheService.RemoveAsync("PendingRequests");
                await _cacheService.RemoveAsync("AcceptedRequests");
                await _cacheService.RemoveAsync("AllRideRequests");

                return Ok(new { Status = "Success", Message = "Ride Request accepted successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("CancelRideRequest/{id:int}")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Canceled a Ride Request", Description = "Canceled a Ride Request by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Trip Canceled successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> CancelRideRequest(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await requestsService.CancelRideRequest(id);
                await _cacheService.RemoveAsync("PendingRequests");
                await _cacheService.RemoveAsync("AllRideRequests");
                return Ok(new { Status = "Success", Message = "Ride Request canceled successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("RejectRideRequest/{id:int}")]
        [SwaggerOperation(Summary = "Reject a Ride Request", Description = "Reject a Ride Request by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Rejected successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> RejectRideRequest(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await requestsService.RejectRideRequest(id);
                await _cacheService.RemoveAsync("PendingRequests");
                await _cacheService.RemoveAsync("AllRideRequests");
                return Ok(new { Status = "Success", Message = "Ride Request rejected successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAcceptedRequests")]
        [SwaggerOperation(Summary = "Get List of Accepted Requests", Description = "Get All Requests Accepted ")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of Accepted Requests ")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetAcceptedRequests()
        {
            string cacheKey = "AcceptedRequests";
            var cached = await _cacheService.GetAsync<List<RideRequestListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.GetAcceptedRequests() ;
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }

            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetCompletedRequests")]
        [SwaggerOperation(Summary = "Get List of Completed Requests", Description = "Get All Requests Completed ")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of Completed Requests ")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetCompletedRequests()
        {
            string cacheKey = "CompletedRequests";
            var cached = await _cacheService.GetAsync<List<RideRequestListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.GetCompletedRequests() ;
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));
                return Ok(Result);
            }

            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
              
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("GetPendingRequests")]
        [SwaggerOperation(Summary = "Get List of Pending Requests", Description = "Get All Requests Pending ")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of Pending Requests ")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetPendingRequests()
        {
            string cacheKey = "PendingRequests";
            var cached = await _cacheService.GetAsync<List<RideRequestListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.GetPendingRequests() ;
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));
                return Ok(Result);
            }

            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        } 
        [Authorize(Roles = "Admin")]
        [HttpGet("ListRideRequests")]
        [SwaggerOperation(Summary = "Get List of  Requests", Description = "Get All Requests  ")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of  Requests ")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> ListRideRequests()
        {
            string cacheKey = "AllRideRequests";
            var cached = await _cacheService.GetAsync<List<RideRequestListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.ListRideRequests() ;
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));
                return Ok(Result);
            }

            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("RideRequestDetails/{id:int}")]
        [SwaggerOperation(Summary = "Get a Ride Request ", Description = "Get a Ride Request by its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> RideRequestDetails(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id Must Be Greater Than 0!!!!!!!!!!!!!! ");
            }
            string cacheKey = $"RideRequest_{id}";
            var cached = await _cacheService.GetAsync<RideRequestDetailsDTO>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.RideRequestDetails(id);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }

        [HttpGet("SearchRideRequests")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get a Ride Request ", Description = "Get a Ride Request by its search.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Gated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]

        public async Task<IActionResult> SearchRideRequests([FromQuery]SearchRideRequestDTO search)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string cacheKey = $"RideRequestSearch_{search.RiderEmail}_{search.Status}";
            var cached = await _cacheService.GetAsync<List<RideRequestListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await requestsService.SearchRideRequests(search);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }


        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRideRequest/{id:int}")]
        [SwaggerOperation(Summary = "Update a Ride Request ", Description = "Update a Ride Request by id.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Ride Request ID.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]


        public async Task<IActionResult> UpdateRideRequest(int id, UpdateRideRequestDTO requestDTO)
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
                var Result = await requestsService.UpdateRideRequest(id,requestDTO);
                await _cacheService.RemoveAsync($"RideRequest_{id}");
                await _cacheService.RemoveAsync("AllRideRequests");
                await _cacheService.RemoveAsync("PendingRequests");
                await _cacheService.RemoveAsync("AcceptedRequests");
                await _cacheService.RemoveAsync("CompletedRequests");
                return Ok(Result);


            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }



    }
}
