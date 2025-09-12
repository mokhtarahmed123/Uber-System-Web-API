using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain;
using Uber.Uber.Domain.DTOs;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;


namespace Uber.Uber.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UberContext contex;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<RoleApp> roleManager;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly IUserRepo user;

        public UserService(IHttpContextAccessor httpContextAccessor ,UberContext contex,
            UserManager<User> userManager,
            IConfiguration configuration,
            RoleManager<RoleApp> roleManager, IUserRepo userRepo,IMapper mapper , IEmailService emailService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.contex = contex;
            this.userManager = userManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        public async Task EnsureRoleExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return;
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new RoleApp { Name = roleName });
            }

        }
        public async Task<string> SignUpasCustomer(SignUpDTOAsCustomer signUpDTO)
        {
            if (signUpDTO == null ||
                           string.IsNullOrWhiteSpace(signUpDTO.Name) ||
                           string.IsNullOrWhiteSpace(signUpDTO.Email) ||
                           string.IsNullOrWhiteSpace(signUpDTO.Phone) ||
                           string.IsNullOrWhiteSpace(signUpDTO.Password))
            {
                throw new ArgumentNullException("Please fill all required fields.");
            }

            await EnsureRoleExistsAsync("Admin");
            await EnsureRoleExistsAsync("Customer");


            var adminRole = await contex.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
                throw new NotFoundException("Admin role not found.");

            var hasAnyAdmin = await userManager.Users.AnyAsync(u => u.RoleId == adminRole.Id);
            if (!hasAnyAdmin)
            {

                if (await userManager.FindByEmailAsync(signUpDTO.Email) != null)
                    throw new BadRequestException($"A user with email '{signUpDTO.Email}' already exists.");

                var adminUser = new User
                {
                    Name = signUpDTO.Name,
                    Email = signUpDTO.Email,
                    PhoneNumber = signUpDTO.Phone,
                    UserName = signUpDTO.Email,
                    RoleId = adminRole.Id
                };

                var result = await userManager.CreateAsync(adminUser, signUpDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Failed to create admin: {errors}");
                }

                await userManager.AddToRoleAsync(adminUser, "Admin");
                return "Admin Created Successfully!";
            }


            var customerRole = await contex.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            if (customerRole == null)
                throw new NotFoundException("Customer role not found");

            if (await userManager.FindByEmailAsync(signUpDTO.Email) != null)
                throw new BadRequestException($"A user with email '{signUpDTO.Email}' already exists.");

            var customer = new User
            {
                Name = signUpDTO.Name,
                Email = signUpDTO.Email,
                PhoneNumber = signUpDTO.Phone,
                RoleId = customerRole.Id,
                UserName = signUpDTO.Email
            };

            var res2 = await userManager.CreateAsync(customer, signUpDTO.Password);
            if (!res2.Succeeded)
            {
                var errors = string.Join(", ", res2.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            await userManager.AddToRoleAsync(customer, "Customer");

            var Cust = new Customer
            {
                Address = signUpDTO.Address,
                City = signUpDTO.City,
                Region = signUpDTO.Region,
                AppUserId = customer.Id,
                
            };
            await contex.Customers.AddAsync(Cust);
            await contex.SaveChangesAsync();

            return "Customer Created";
        }

        public async Task<string> SignUpasMerchant(SignUpasMerchant signUpDTO)
        {

            if (signUpDTO == null ||
                         string.IsNullOrWhiteSpace(signUpDTO.Name) ||
                         string.IsNullOrWhiteSpace(signUpDTO.Email) ||
                         string.IsNullOrWhiteSpace(signUpDTO.Phone) ||
                         string.IsNullOrWhiteSpace(signUpDTO.Password) ||
                         string.IsNullOrWhiteSpace(signUpDTO.Address) ||
                         signUpDTO.Latitude < -90 || signUpDTO.Latitude > 90 ||
                         signUpDTO.Longitude < -180 || signUpDTO.Longitude > 180)
            {
                throw new ArgumentNullException("Please fill all required fields.");
            }

            await EnsureRoleExistsAsync("Merchant");

            var existingMerchant = await contex.Merchants.Include(m => m.UserApp)
                                                           .FirstOrDefaultAsync(m => m.UserApp.Email == signUpDTO.Email);
            if (existingMerchant != null)
                throw new BadRequestException($"Merchant with email {signUpDTO.Email} already exists.");

            if (await userManager.FindByEmailAsync(signUpDTO.Email) != null)
                throw new BadRequestException($"A user with email '{signUpDTO.Email}' already exists.");

            var user = new User
            {
                Email = signUpDTO.Email,
                Name = signUpDTO.Name,
                PhoneNumber = signUpDTO.Phone,
                RoleId = (await contex.Roles.FirstAsync(r => r.Name == "Merchant")).Id,
                UserName = signUpDTO.Email
            };

            var result = await userManager.CreateAsync(user, signUpDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create User: {errors}");
            }

            await userManager.AddToRoleAsync(user, "Merchant");

            var merchant = new Merchant
            {
                Address = signUpDTO.Address,
                Longitude = signUpDTO.Longitude,
                Latitude = signUpDTO.Latitude,
                AppUserId = user.Id
            };

            await contex.Merchants.AddAsync(merchant);
            await contex.SaveChangesAsync();

            return "Merchant Created";
        }

        public async Task<string> SignUpDTOAsDriver(SignUpDTOAsDriver signUpDTO)
        {
            if (signUpDTO == null ||
                string.IsNullOrWhiteSpace(signUpDTO.Name) ||
                string.IsNullOrWhiteSpace(signUpDTO.Email) ||
                string.IsNullOrWhiteSpace(signUpDTO.Phone) ||
                string.IsNullOrWhiteSpace(signUpDTO.Password) ||
                string.IsNullOrWhiteSpace(signUpDTO.VehicleType) ||
                string.IsNullOrWhiteSpace(signUpDTO.Status.ToString()))
            {
                throw new ArgumentNullException("Please fill all required fields.");
            }

            await EnsureRoleExistsAsync("Delivery");

            var existingDriver = await contex.DriverProfiles.Include(d => d.user)
                                                              .FirstOrDefaultAsync(d => d.user.Email == signUpDTO.Email);
            if (existingDriver != null)
                throw new BadRequestException($"Driver with email {signUpDTO.Email} already exists.");

            if (await userManager.FindByEmailAsync(signUpDTO.Email) != null)
                throw new BadRequestException($"A user with email '{signUpDTO.Email}' already exists.");

            var user = new User
            {
                Email = signUpDTO.Email,
                Name = signUpDTO.Name,
                PhoneNumber = signUpDTO.Phone,
                UserName = signUpDTO.Email,
                RoleId = (await contex.Roles.FirstAsync(r => r.Name == "Delivery")).Id
            };

            var result = await userManager.CreateAsync(user, signUpDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create Driver: {errors}");
            }

            await userManager.AddToRoleAsync(user, "Delivery");

            var driver = new DriverProfile
            {
                LicenseImagePath = signUpDTO.Image?.ToString(), // ensure null-safe
                DriverID = user.Id,
                VehicleType = signUpDTO.VehicleType,
                PlateNumber = signUpDTO.PlateNumber,
                Status = signUpDTO.Status
            };

            await contex.DriverProfiles.AddAsync(driver);
            await contex.SaveChangesAsync();

            return "Driver Created";


        }
        public async Task<bool> ChangePassword(ChangePasswordAsync loginDTO)
        {
            if (loginDTO == null ||
                string.IsNullOrWhiteSpace(loginDTO.Email) ||
                string.IsNullOrWhiteSpace(loginDTO.Password) ||
                string.IsNullOrWhiteSpace(loginDTO.NewPassword))
            {
                throw new ArgumentNullException("Please fill all required fields.");
            }

            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                throw new NotFoundException($"User with email {loginDTO.Email} not found.");

            var isOldPasswordCorrect = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isOldPasswordCorrect)
                throw new BadRequestException("Old password is incorrect.");

            var result = await userManager.ChangePasswordAsync(user, loginDTO.Password, loginDTO.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Password change failed: {errors}");
            }

            return true;


        }

        public async Task<bool> DeleteUser(string email) // From Admin
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email), "Please provide a valid email.");
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException($"User with email '{email}' was not found.");

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to delete user: {errors}");
            }

            return true;


        }

        public async Task<bool> ForgetPassword(ForgetPasswordDto Pass)
        {
            if(!string.IsNullOrWhiteSpace(Pass.Email))
                throw new ArgumentNullException(nameof(Pass.Email), "Please Enter All Fieldes");
            var user = await userManager.FindByEmailAsync(Pass.Email);
            if (user == null)
                throw new NotFoundException($"User with email {Pass.Email} not found.");
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://localhost:5105/api/user/resetpassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            string subject = "Reset your Uber password";
            string body = $"<h3>Hello {user.UserName},</h3><p>Click below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";

            await emailService.SendEmailAsync(user.Email, subject, body);

            return true;
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {

            var users = await contex.Users.Include(a => a.Role).ToListAsync();

            var userDtos = users
                .Where(u => u != null)
                .Select(u => new UserDTO
                {
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.PhoneNumber
                })
                .ToList();

            return userDtos;
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "Please enter an email.");

            var found = await userManager.FindByEmailAsync(email);
            if (found == null)
                throw new NotFoundException($"User with email {email} not found.");


            var user = new UserDTO
            {
                Name = found.Name,
                Phone = found.PhoneNumber,
                Email = found.Email,
            };
            return user;
        }

        public async Task<string> Login(LoginDTO login)
        {
            if (login == null ||
        string.IsNullOrWhiteSpace(login.Email) ||
            string.IsNullOrWhiteSpace(login.Password))
            {
                throw new ArgumentNullException("Please fill all required fields.");
            }
            var IsFound = await userManager.FindByEmailAsync(login.Email);
            if (IsFound == null) throw new System.UnauthorizedAccessException(" User Email Or Password Invalid ");

            var CheckPassword = await userManager.CheckPasswordAsync(IsFound, login.Password);
            if (!CheckPassword)
                throw new BadRequestException(" Password Wrong, Try Again ");

            var RoleId = IsFound.RoleId;

            var RoleFromUser = await userManager.GetRolesAsync(IsFound);
            if (RoleFromUser == null || !RoleFromUser.Any())
                throw new BadRequestException("This user has no roles assigned.");

            var Claims = new List<Claim>
                 {
           new Claim(ClaimTypes.Email, login.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
           new Claim(ClaimTypes.Name, IsFound.Name),
        new Claim("UserID", IsFound.Id)
              };

            foreach (var role in RoleFromUser)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT SecretKey is missing from appsettings.json!");
            }
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var Sign = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);


            var Token = new JwtSecurityToken(
                 issuer: configuration["JWT:IssuerIP"],
                audience: configuration["JWT:AudienceIP"],
                expires: DateTime.Now.AddHours(5),
                claims: Claims,
                signingCredentials: Sign
           );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(Token);
            return tokenString;
        }


        public async Task<bool> UpdateProfileAsync(UpdateUserDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Phone) ||
                string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentNullException(nameof(dto), "Please fill all required fields.");
            }

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new NotFoundException($"User with email '{dto.Email}' not found.");

            mapper.Map(dto, user);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to update profile: {errors}");
            }

            return true;
        }

        public async Task<string> GetUserRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID '{userId}' not found.");


            var roles = await userManager.GetRolesAsync(user);

            if (roles == null || roles.Count == 0)
                return $"User with email {user.Email} has no roles assigned.";

            return $"User with email {user.Email} has the following roles: {string.Join(", ", roles)}";
        }

        public async Task<bool> SendEmailVerification(string Email)
        {

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentNullException(nameof(Email), "Please provide an email.");

            // البحث عن المستخدم
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
                throw new NotFoundException($"User with email {Email} not found.");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);

            var confirmationLink = $"https://localhost:5105/api/user/VerifyEmail?userId={user.Id}&token={encodedToken}";

            string subject = "Verify your email - Uber App";
            string body = $@"
        <h2>Welcome to Uber! 🚖</h2>
        <p>Please confirm your email by clicking the link below:</p>
        <p><a href='{confirmationLink}'>Confirm Email</a></p>
        <br/>
        <p>If you didn't request this, you can ignore this email.</p>";

            await emailService.SendEmailAsync(Email, subject, body);

            return true;
        }

        public async Task<bool> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "Please provide a valid User ID.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found.");

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new BadRequestException("Invalid or expired email confirmation token.");

            string subject = "Welcome to Uber App 🚖";
            string body = $@"
        <h2>Hello {user.UserName}! 🎉</h2>
        <p>Your email has been successfully verified.</p>
        <p>Thanks for joining Uber! 🚀</p>";

            try
            {
                await emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VerifyEmail] Failed to send welcome email: {ex.Message}");
            }

            return true;
        }


            public async Task<bool> SendEmailTOResetPassword(string userEmail)
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                    throw new ArgumentNullException(nameof(userEmail), "Please provide a valid email.");

                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                    throw new NotFoundException($"User with email {userEmail} not found.");
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);

            var confirmationLink = $"https://localhost:5105/api/user/ResetPasswordAsync?userId={user.Id}&token={encodedToken}";

            string subject = "Reset Password Request";
            string body = $@"
        <h2>Hello, {user.UserName} 👋</h2>
        <p>You requested to reset your password.</p>
        <p>Click the link below to reset it:</p>
        <a href='{confirmationLink}'>Reset Password</a>";

            try
                {
                    await emailService.SendEmailAsync(userEmail, subject, body);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmailService] Failed to send confirmation email: {ex.Message}");
                    throw new ApplicationException("Failed to send confirmation email. Please try again later.", ex);
                }
            }

        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException("All fields are required.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found.");

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Password reset failed: {errors}");
            }

            return true;
        }

        public async Task<UserDTO> GetMyProfile()
        {

            var userId = httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new System.UnauthorizedAccessException("Invalid token");

            var user = await userManager.FindByIdAsync(userId);
            var User = new UserDTO
            {
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber

            };
            return User;
        }

        
    }
}
