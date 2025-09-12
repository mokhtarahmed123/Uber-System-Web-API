using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application.DTOs;
using Uber.Uber.Application.DTOs.DriverProfileDTOS;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Application.Interfaces.Repository.DriverProfiles;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application.Services
{
    public class DriverProfileService : IDriverProfileService
    {
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly ILogger<DriverProfile> logger;
        private readonly IDriverProfilesRepo profilesRepo;
        private readonly UserManager<User> usermanger;

        public DriverProfileService( UberContext context , IMapper mapper ,ILogger<DriverProfile> logger , IDriverProfilesRepo profilesRepo , UserManager<User> usermanger )
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
            this.profilesRepo = profilesRepo;
            this.usermanger = usermanger;
        }

        public async Task<bool> ChangeStatusAsync(int id, ChangeDriverStatusDTO changeDriverStatusDTO)
        {
            if (id <= 0)
                throw new BadRequestException("Id must be greater than 0");

            var driverProfile = await profilesRepo.GetByID(id);
            if (driverProfile == null)
                throw new NotFoundException($"Driver Profile with id {id} not found");

            driverProfile.Status = changeDriverStatusDTO.Status;

            var updated = await profilesRepo.Update(id, driverProfile);
            if (updated == null)
            {
                logger.LogError($"Failed to update status for Driver Profile ID: {id}");
                throw new BadRequestException("Failed to update driver status");
            }

            logger.LogInformation($"Driver Profile with ID {id} status updated successfully to {driverProfile.Status}");
            return true;
        }

        

        public async Task<CreateDriverProfile> CreateAsync(CreateDriverProfile createDriverProfile)
        {
           if (createDriverProfile == null)
            {
                logger.LogError("Please Enter All Fieldes  ");
                throw new ArgumentNullException("Please Enter All Fieldes ");
            }
            var user = await usermanger.FindByEmailAsync(createDriverProfile.DriverEmail); 
            if (user == null)
            {
                logger.LogError($"User with Email {createDriverProfile.DriverEmail} not found.");
                throw new NotFoundException($"User with Email {createDriverProfile.DriverEmail} not found.");
            }


            var Driverisfound = await context.DriverProfiles.Include(a=>a.user)
                .FirstOrDefaultAsync(dp => dp.DriverID == user.Id);
            if (Driverisfound != null) {
                logger.LogError($" Driver Profile with Email {createDriverProfile.DriverEmail} Not Found ");
                throw new NotFoundException($" Driver Profile with Email {createDriverProfile.DriverEmail} Not Found ");
            }

            var Mapped = mapper.Map<DriverProfile>(createDriverProfile);
             await profilesRepo.Create(Mapped);
            logger.LogInformation($"Driver Profile Added SuccessFully !");
            return mapper.Map<CreateDriverProfile>(Mapped);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                logger.LogError(" Id Must Be Greater Than 0 ");
                throw new BadRequestException("Id Must Be Greater Than 0");
            }

           var IsFound = await profilesRepo.GetByID(id);
            if (IsFound == null)
            {
                logger.LogError($" Driver Profile with id {id} Not Found ");
                throw new NotFoundException($" Driver Profile with id {id} Not Found ");
            }
             await profilesRepo.Delete(id);
            logger.LogInformation($"Driver Profile Deleted SuccessFully !");
            return true;
        }

        public async Task<List<GetAllDriversDTO>> GetAllAsync()
        {
            var List = await profilesRepo.FindAll();
            return mapper.Map<List<GetAllDriversDTO>>(List);
        }

        public async Task<GetDriverProfilesDetails> GetDetailsByEmailAsync(string driverEmail)
        {
            if (string.IsNullOrWhiteSpace(driverEmail))
            {
                logger.LogError("Driver email cannot be empty");
                throw new BadRequestException("Driver email cannot be empty");
            }

            var IsFound = await context.DriverProfiles.Include(a => a.user).FirstOrDefaultAsync(a => a.user.Email == driverEmail);
            if(IsFound == null)
            {
                logger.LogError($" Driver Profile with Email {driverEmail} Not Found ");
                throw new NotFoundException($" Driver Profile with Email {driverEmail} Not Found ");
            }
            var Mapped = mapper.Map<GetDriverProfilesDetails>(IsFound);

            return Mapped;

        }

        public async Task<UpdateDriverProfile> UpdateAsync(int id, UpdateDriverProfile updateDriverProfile)
        {
            if (id <= 0)
            {
                logger.LogError(" Id Must Be Greater Than 0 ");
                throw new BadRequestException("Id Must Be Greater Than 0");
            }
            var IsFound = await profilesRepo.GetByID(id);
            if (IsFound == null) {
                logger.LogError($" Driver Profile with id {id} Not Found ");
                throw new NotFoundException($" Driver Profile with id {id} Not Found ");
            }
            if (updateDriverProfile == null) {
                logger.LogError("Please Enter All Fieldes  ");
                throw new ArgumentNullException("Please Enter All Fieldes ");
            }

            var Mapped = mapper.Map(updateDriverProfile,IsFound);

            var Updated = await profilesRepo.Update(id, Mapped);
            if (Updated == null) {
                logger.LogError("Failed to update driver status.");
                throw new BadRequestException("Failed to update driver status.");
            }
            logger.LogInformation($"Driver Profile with ID {id} status updated to {IsFound.Status}");
            return mapper.Map<UpdateDriverProfile>(Updated);

        }
    }
}
