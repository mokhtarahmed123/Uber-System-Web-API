using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application.DTOs.ReviewsDTOs;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService reviewsService;
        private readonly ICacheService _cacheService;

        public ReviewsController(IReviewsService reviewsService , ICacheService cacheService)
        {
            this.reviewsService = reviewsService;
            this._cacheService = cacheService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Check Reviews API health", Description = "Returns a simple message to confirm Reviews API is working.")]
        [SwaggerResponse(200, "API is up and running")]
        public IActionResult Index()
        {
            return Ok(" Reviews API is working. ");
        }
        [Authorize(Roles = "Customer,Admin,Merchant,Driver")]
        [HttpPost("AddReview")]
        [SwaggerOperation(Summary = "Add a new review", Description = "Creates a new review for a specific trip, customer, or driver.")]
        [SwaggerResponse(200, "Review created successfully")]
        [SwaggerResponse(400, "Invalid review data")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDTO createReviewDTO)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try
            {
                var Result = await reviewsService.CreateReviewAsync(createReviewDTO);
                await _cacheService.RemoveAsync("AllReviews");
                await _cacheService.RemoveAsync($"DriverAvg_{createReviewDTO.DriverEmail}");
                return Ok(Result);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Error: {ex.Message}");

            }

        }
        [Authorize(Roles = "Customer,Admin,Merchant,Driver")]
        [HttpDelete("DeleteReview/{id:int}")]
        [SwaggerOperation(Summary = "Delete review", Description = "Deletes an existing review by ID.")]
        [SwaggerResponse(200, "Review deleted successfully")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Result = await reviewsService.DeleteReviewAsync(id);
                await _cacheService.RemoveAsync("AllReviews");

                return Ok(Result);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin,Driver")]

        [HttpGet("GetById/{id:int}")]
        [SwaggerOperation(Summary = "Get review by ID", Description = "Retrieves a specific review by its ID.")]
        [SwaggerResponse(200, "Review retrieved successfully")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var Result = await reviewsService.GetByIdAsync(id);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all reviews", Description = "Retrieves a list of all reviews in the system.")]
        [SwaggerResponse(200, "Reviews retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "AllReviews";
            var cached = await _cacheService.GetAsync<List<ReviewDetailsDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try {
                var Result = await reviewsService.GetAll();
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("GetCustomerReviewsAsync/{Email}")]
        [SwaggerOperation(Summary = "Get customer reviews", Description = "Retrieves all reviews submitted by a specific customer.")]
        [SwaggerResponse(200, "Customer reviews retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]

        public async Task<IActionResult> GetCustomerReviews(string Email)
        {
            string cacheKey = $"CustomerReviews_{Email}";
            var cached = await _cacheService.GetAsync<List<ReviewDetailsDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await reviewsService.GetCustomerReviewsAsync(Email);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }

        }
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("DriverReviewsAsync/{Email}")]
        [SwaggerOperation(Summary = "Get driver reviews", Description = "Retrieves all reviews for a specific driver.")]
        [SwaggerResponse(200, "Driver reviews retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> DriverReviews(string Email)
        {
            string cacheKey = $"DriverReviews_{Email}";
            var cached = await _cacheService.GetAsync<List<ReviewDetailsDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await reviewsService.DriverReviewsAsync(Email);
                await _cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,Driver")]

        [HttpGet("GetDriverAverageRatingAsync/{Email}")]
        [SwaggerOperation(Summary = "Get driver's average rating", Description = "Calculates and returns the average rating for a specific driver.")]
        [SwaggerResponse(200, "Driver average rating calculated successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetDriverAverageRating(string Email)
        {

            try
            {
                var Result = await reviewsService.GetDriverAverageRatingAsync(Email);
                return Ok(new { Email = Email, Average =  Result});

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
         }
        [Authorize(Roles = "Admin,Customer")]

        [HttpGet("GetCustomerReviewsCountAsync/{Email}")]
        [SwaggerOperation(Summary = "Get customer reviews count", Description = "Returns the total number of reviews submitted by a specific customer.")]
        [SwaggerResponse(200, "Customer reviews count retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetCustomerReviewsCount(string Email)
        {
            try
            {
                var Result = await reviewsService.GetCustomerReviewsCountAsync(Email);
                return Ok(new { Email = Email, Count = Result });

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("GetDriverReviewsCountAsync/{Email}")]
        [SwaggerOperation(Summary = "Get driver reviews count", Description = "Returns the total number of reviews for a specific driver.")]
        [SwaggerResponse(200, "Driver reviews count retrieved successfully")]
        [SwaggerResponse(500, "Unexpected server error")]
        public async Task<IActionResult> GetDriverReviewsCount(string Email)
        {
            try
            {
                var Result = await reviewsService.GetDriverReviewsCountAsync(Email);
                return Ok(new { Email = Email, Count = Result });

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Customer,Admin,Driver,Merchant")]
        [HttpPut("UpdateReviewAsync/{id:int}")]
        [SwaggerOperation(Summary = "Update review", Description = "Updates an existing review by ID.")]
        [SwaggerResponse(200, "Review updated successfully")]
        [SwaggerResponse(400, "Invalid review data")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "Unexpected server error")]

        public async Task<IActionResult> UpdateReviewAsync(int id, UpdateReviewDTO updateReviewDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var Result = await reviewsService.UpdateReviewAsync(id, updateReviewDTO);
                await _cacheService.RemoveAsync("AllReviews");

                return Ok(Result);
            }

            catch (Exception ex)
            {

                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


    }
}
