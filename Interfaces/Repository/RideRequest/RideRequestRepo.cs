using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application
{
    public class RideRequestRepo : IRideRequestRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Item> logger;
        private readonly IMapper mapper;

        public RideRequestRepo(UberContext context, ILogger<Item> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Create(RideRequest entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.RideRequests.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Ride Request Added Successfully ");
        }

        public async Task Delete(int id)
        {
            var isfound = await context.RideRequests.FindAsync(id);
            if (isfound != null)
            {
                context.RideRequests.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" Ride Request  Deleted Successfully ");
                return;
            }
            logger.LogError($" Ride Request With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Ride Request With ID {id} Not Found , try Again  ");
        }

        public async Task<List<RideRequest>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.RideRequests.Include(A=>A.Rider)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .ToListAsync();
        }

        public async Task<RideRequest> GetByID(int id)
        {
            var isfound = await context.RideRequests.Include(A => A.Rider).FirstOrDefaultAsync(i=>i.ID == id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" Ride Request With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Ride Request With ID {id} Not Found , try Again  ");

        }


        public async Task<RideRequest> Update(int id, RideRequest entity)
        {
            var existing = await context.RideRequests.FindAsync(id);
            if (existing == null)
            {
                logger.LogError($"Ride Request With ID {id} Not Found");
                throw new NotFoundException($"Ride Request With ID {id} Not Found");
            }

            context.Entry(existing).CurrentValues.SetValues(entity);

            try
            {
                await SaveChange();
                logger.LogInformation("Ride Request Updated Successfully");
            }
            catch (DbUpdateException ex)
            {
                logger.LogError($"Error while saving ride request: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }

            return existing;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }
    }
}
