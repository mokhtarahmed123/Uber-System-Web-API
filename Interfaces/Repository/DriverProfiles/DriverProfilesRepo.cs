
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application.Interfaces.Repository.DriverProfiles
{
    public class DriverProfilesRepo : IDriverProfilesRepo
    {
        private readonly UberContext context;
        private readonly ILogger<DriverProfile> logger;
        public DriverProfilesRepo(UberContext context, ILogger<DriverProfile> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task Create(DriverProfile entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.DriverProfiles.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" DriverProfile Added Successfully ");

        }

        public async Task Delete(int id)
        {
            var isfound = await context.DriverProfiles.FindAsync(id);
            if (isfound != null)
            {
                context.DriverProfiles.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" DriverProfile Deleted Successfully ");
                return;
            }
            logger.LogError($" DriverProfile With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" DriverProfile With ID {id} Not Found , try Again  ");
        }

        public async Task<List<DriverProfile>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.DriverProfiles.Include(a=>a.user).Include(a=>a.Trips).Include(a=>a.reviews).Include(a=>a.Deliveries)
                .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                             .ToListAsync();
        }

        public async Task<DriverProfile> GetByID(int id)
        {
            var isfound = await context.DriverProfiles.FindAsync(id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" DriverProfile With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" DriverProfile With ID {id} Not Found , try Again  ");
        }


        public async Task<DriverProfile> Update(int id, DriverProfile entity)
        {
            var isfound = await context.DriverProfiles.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($" Item With ID {id} Not Found , try Again  ");
                throw new NotFoundException($" Item With ID {id} Not Found , try Again  ");
            }
            isfound.PlateNumber = entity.PlateNumber;
            isfound.LicenseImagePath = entity.LicenseImagePath;
            isfound.DriverID = entity.DriverID;
            isfound.VehicleType = entity.VehicleType;
            isfound.Status = entity.Status;
            await SaveChange();
            logger.LogInformation(" Items Updated Successfully ");
            return isfound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }
    }
}
