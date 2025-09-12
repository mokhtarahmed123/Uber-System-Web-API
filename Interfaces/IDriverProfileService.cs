using Uber.Uber.Application.DTOs;
using Uber.Uber.Application.DTOs.DriverProfileDTOS;

namespace Uber.Uber.Application
{
    public interface IDriverProfileService
    {
        Task<CreateDriverProfile> CreateAsync(CreateDriverProfile createDriverProfile);

        Task<UpdateDriverProfile> UpdateAsync(int id, UpdateDriverProfile updateDriverProfile);

        Task<bool> DeleteAsync(int id);

        Task<GetDriverProfilesDetails> GetDetailsByEmailAsync(string driverEmail);

        Task<List<GetAllDriversDTO>> GetAllAsync();

        Task<bool> ChangeStatusAsync(int id,ChangeDriverStatusDTO changeDriverStatusDTO);

    }
}
