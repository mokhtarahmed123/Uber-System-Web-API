using Uber.Uber.Domain;
using Uber.Uber.Domain.DTOs;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public interface IUserService
    {
        Task<string> SignUpasCustomer(SignUpDTOAsCustomer signUpDTO);
        Task<string> SignUpasMerchant(SignUpasMerchant signUpDTO);
        Task<string> SignUpDTOAsDriver(SignUpDTOAsDriver signUpDTO);
        Task<string> Login(LoginDTO login);

         Task<bool> ForgetPassword(ForgetPasswordDto Pass);
        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<bool> UpdateProfileAsync(UpdateUserDTO dto);
        Task<bool> ChangePassword(ChangePasswordAsync loginDTO);
        Task<List<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserByEmail(string Email);
        Task<bool> DeleteUser(string Email);
        Task<string> GetUserRoles(string userid); 
        Task<bool> SendEmailVerification(string Email);
        Task<bool> VerifyEmail(string userId, string token);
        Task<bool> SendEmailTOResetPassword(string userEmail);
        Task<UserDTO> GetMyProfile();
        Task EnsureRoleExistsAsync(string roleName);



    }
}
