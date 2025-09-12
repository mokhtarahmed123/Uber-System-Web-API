using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uber.Uber.Domain;
using Uber.Uber.Domain.DTOs;
using Uber.Uber;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Uber.Uber.Domain.Exceptions;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEmailService emailService;
        private readonly IUserService userService;
        private readonly ICacheService cacheService;

        public UserController(IEmailService emailService,IUserService userService, ICacheService cacheService)
        {
            this.emailService = emailService;
            this.userService = userService;
            this.cacheService = cacheService;
        }
        #region Index
        [SwaggerOperation(Summary = "Check if User API is working")]
        [SwaggerResponse(200, "User API is working.")]
        [HttpGet]
        public ActionResult Index()
        {
            return Ok(" User Api Is Working  ");
        }
        #endregion
        #region SignUpasCustomer
        [SwaggerOperation(
            Summary = "Creates a new Customer ",
            Description = "This endpoint allows you to create a new User and store it in the database."
        )]
        [SwaggerResponse(201, "User Added Successfully")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost("SignUpasCustomer")]

        public async Task<IActionResult> SignUpasCustomer([FromBody] SignUpDTOAsCustomer signUpDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(signUpDTO);
            }
            try
            {
                var Result = await userService.SignUpasCustomer(signUpDTO);
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }


        #endregion
        #region SignUpasDriver
        [SwaggerOperation(
            Summary = "Creates a new Driver ",
            Description = "This endpoint allows you to create a new Driver and store it in the database."
        )]
        [SwaggerResponse(201, "Driver Added Successfully")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost("SignUpasDriver")]
        public async Task<IActionResult> SignUpasDriver([FromForm] SignUpDTOAsDriver signUpDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(signUpDTO);
            }
            try
            {
                var Result = await userService.SignUpDTOAsDriver(signUpDTO);
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        #endregion
        #region SignUpasMerchant
        [SwaggerOperation(
            Summary = "Creates a new Merchant ",
            Description = "This endpoint allows you to create a new Merchant and store it in the database."
        )]
        [SwaggerResponse(201, "Merchant Added Successfully")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost("SignUpasMerchant")]

        public async Task<IActionResult> SignUpasMerchant([FromBody] SignUpasMerchant signUpDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(signUpDTO);
            }
            try
            {
                var Result = await userService.SignUpasMerchant(signUpDTO);
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        #endregion
        #region Login

        [SwaggerOperation( Summary = "Log in  User", Description = "This endpoint allows you to Log In  .")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost("Login")]

        public async Task<IActionResult> Login(LoginDTO dTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var Result = await  userService.Login(dTO);
                return Ok(Result);
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        #endregion
        #region Delete User (Admin Only)
        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete user", Description = "Deletes a user by email.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromQuery] string email)
        {
            try
            {
                var result = await userService.DeleteUser(email);
                await cacheService.RemoveAsync($"user_{email}");
                await cacheService.RemoveAsync("all_users");
                return Ok(new { Status = "Success", Message = "User deleted", Data = result });
            
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        #endregion
        #region GetUserByEmail
        [HttpGet("GetUserByEmail")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get a User ", Description = "Get a User by its Email .")]
        [SwaggerResponse(StatusCodes.Status200OK, "User Returned successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User Email  .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]

        public async Task<IActionResult> GetUserByEmail([FromQuery]string Email)
        {
            string cacheKey = $"user_{Email}";
            var cached = await cacheService.GetAsync<object>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await userService.GetUserByEmail(Email);
                await cacheService.SetAsync(cacheKey, new { Status = "Success", Message = "User retrieved", Data = Result }, TimeSpan.FromMinutes(10));

                return Ok(Result);

            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        #endregion
        #region Get All Users (Admin Only)
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get all users", Description = "Fetch all users.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            string cacheKey = "all_users";
            var cached = await cacheService.GetAsync<object>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var result = await userService.GetAllUsers();
                await cacheService.SetAsync(cacheKey, new { Status = "Success", Message = "Users retrieved", Data = result }, TimeSpan.FromMinutes(10));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        #endregion
        #region UpdateProfileAsync
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin,Customer,Merchant,Delivery")]
        [SwaggerOperation(Summary = "Update a User ", Description = "Update a User by its Email .")]
        [SwaggerResponse(StatusCodes.Status200OK, "User Updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User Email  .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]

        public async Task<IActionResult> UpdateProfileAsync(UpdateUserDTO dto)
        {
            try
            {
                var Result = await userService.UpdateProfileAsync(dto);

                await cacheService.RemoveAsync($"user_{dto.Email}");
                await cacheService.RemoveAsync("all_users");
                return Ok(new { Status = "Success", Message = "User updated successfully", Data = Result });

            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }
        #endregion
        #region Get User Roles
        [HttpGet("GetUserRoles")]
        [Authorize]
        [SwaggerOperation(Summary = "Get  a User  ", Description = "Get a Role OF User by its User ID {From Token} .")]
        [SwaggerResponse(StatusCodes.Status200OK, "User Updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User ID  .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]

        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var Result = await userService.GetUserRoles(userId);
                return Ok(new { Roles = Result });

            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }

        #endregion
        #region Send Email Verification
        [HttpPost("SendEmailVerification")]
        //[AllowAnonymous]
        [SwaggerOperation(Summary = "Send verification email", Description = "Sends an email with a confirmation link.")]
        public async Task<IActionResult> SendEmailVerification([FromQuery] string email)
        {
            try
            {
                var result = await userService.SendEmailVerification(email);
                return Ok(new { Message = result,   });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        #endregion
        #region VerifyEmail
        [HttpPost("VerifyEmail")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get  a User  ", Description = "Get a Role OF User by its User ID {From Token} .")]
        [SwaggerResponse(StatusCodes.Status200OK, "User Updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User ID  .")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error.")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userId,[FromQuery] string token)
        {
            try
            {
                var result = await userService.VerifyEmail(userId, token);

                if (result)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Email confirmed successfully. Welcome to Uber! 🚖"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to confirm email."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }

        #endregion
        #region SendConfirmationEmail
        [HttpPost("SendEmailTOResetPassword")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Send Confirmation Email", Description = "Sends confirmation email after registration.")]
        public async Task<IActionResult> SendEmailTOResetPassword([FromQuery]string userEmail)
        {
            try
            {
                var Result = await userService.SendEmailTOResetPassword(userEmail);
                return Ok(new { Message = Result });
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

        }
        #endregion
        #region ResetPasswordAsync
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Reset Password", Description = "Resets user password using token.")]

        public async Task<IActionResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                var Result = await userService.ResetPasswordAsync(userId, token, newPassword);
                return Ok(new { Message = Result });
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }


        }
        #endregion

        [SwaggerOperation(Summary = "Get  a User  ", Description = "Get a Role OF User by its  Token .")]
        [HttpGet("My Profile")]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            string cacheKey = $"profile_{User.Identity.Name}";
            var cached = await cacheService.GetAsync<object>(cacheKey);
            if (cached != null) return Ok(cached);
            try
            {
                var Result = await userService.GetMyProfile();
                await cacheService.SetAsync(cacheKey, new { Status = "Success", Message = "Profile retrieved", Data = Result }, TimeSpan.FromMinutes(10));

                return Ok(new { Message = Result });
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }

            }



    }
}