using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.PaymentDTOs;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly ICacheService cacheService;
        private readonly IHubContext<PaymentHub> paymentHub;
        private readonly UberContext context;

        public PaymentController(IPaymentService paymentService, ICacheService cacheService , IHubContext<PaymentHub> paymentHub , UberContext context)
        {
            this.paymentService = paymentService;
            this.cacheService = cacheService;
            this.paymentHub = paymentHub;
            this.context = context;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Check API Health", Description = "Returns a simple confirmation message to check if the Payment API is working.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment API is working successfully.")]
        public IActionResult Index()
        {
            return Ok("Payment API is working.");
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Create a new payment", Description = "Creates a new payment record based on the provided data.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await paymentService.Create(request);
                await paymentHub.Clients.User(request.CustomerEmail)
                      .SendAsync("ReceiveNewPayment", result.TripID, result.PaymentStatus);
                await cacheService.RemoveAsync("all_payments");
                if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                    await cacheService.RemoveAsync($"payments_customer_{request.CustomerEmail}");
                if (!string.IsNullOrWhiteSpace(request.MerchantEmail))
                    await cacheService.RemoveAsync($"payments_merchant_{request.MerchantEmail}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("Delete/{id:int}")]
        [SwaggerOperation(Summary = "Delete a payment", Description = "Deletes a payment record based on its unique ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Payment ID.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Payment not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0.");

            try
            {
                var result = await paymentService.Delete(id);
                await cacheService.RemoveAsync("all_payments");
                await cacheService.RemoveAsync($"payment_{id}");
                if (!result)
                    return NotFound($"Payment with ID {id} not found.");
                return Ok("Payment deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpPut("Update/{id:int}")]
        [SwaggerOperation(Summary = "Update a payment", Description = "Updates the details of an existing payment record.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Payment ID or data.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Payment not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentDTO request)
        {
            if (id <= 0)
                return BadRequest("Invalid payment ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var CustomerEmail = await context.Payments.Include(a=>a.customer).ThenInclude(a=>a.UserApp).FirstOrDefaultAsync(a=>a.ID == id);

            try
            {
                var result = await paymentService.Update(id, request);


                await cacheService.RemoveAsync("all_payments");
                await paymentHub.Clients.Group("Admins")
                    .SendAsync("PaymentUpdate", id, result.PaymentStatus);

                await paymentHub.Clients.User(CustomerEmail.customer.UserApp.Email)
                    .SendAsync("PaymentStatusUpdated", id, result.PaymentStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all payments", Description = "Fetches a list of all payment records.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of all payments.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "all_payments";
            var cached = await cacheService.GetAsync<List<PaymentListDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var result = await paymentService.GetAllAsync();
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("GetById/{id:int}")]
        [SwaggerOperation(Summary = "Get payment by ID", Description = "Fetches a specific payment by its unique ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payment found successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Payment ID.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Payment not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid payment ID.");
            string cacheKey = $"payment_{id}";
            var cached = await cacheService.GetAsync<PaymentDetailsDTO>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var result = await paymentService.GetByIdAsync(id);
                await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("SearchPayments")]
        [SwaggerOperation(Summary = "Search payments", Description = "Searches for payments based on filters like amount, date, or email.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Payments matching the criteria.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid search parameters.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> SearchPayments([FromQuery] SearchPaymentDTO search)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await paymentService.SearchPaymentsAsync(search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,Customer")]

        [HttpGet("GetPaymentsByCustomerEmail/{Email}")]
        [SwaggerOperation(Summary = "Get Payments By  Customer Email", Description = "Get Payments By  Customer Email.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of Payments  Returned.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Email Customer .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetPaymentsByCustomerEmail( string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string cacheKey = $"payments_customer_{Email}";
            var cached = await cacheService.GetAsync<List<PaymentDetailsDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await paymentService.GetPaymentDetailByCustomerEmail(Email);
                await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }       
        [Authorize(Roles = "Admin,Merchant")]

        [HttpGet("GetPaymentsByMerchantEmail/{Email}")]
        [SwaggerOperation(Summary = "Get Payments By  Merchant Email", Description = "Get Payments By  Merchant Email.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List Of Payments  Returned.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Email Merchant .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> GetPaymentsByMerchantEmail( string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string cacheKey = $"payments_merchant_{Email}";
            var cached = await cacheService.GetAsync<List<PaymentDetailsDTO>>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await paymentService.GetPaymentDetailByMerchantEmail(Email);
                await cacheService.SetAsync(cacheKey, Result, TimeSpan.FromMinutes(10));

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
