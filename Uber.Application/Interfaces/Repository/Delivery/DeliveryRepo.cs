using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application
{
    public class DeliveryRepo : IDeliveryRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Delivery> logger;
        private readonly IMapper mapper;

        public DeliveryRepo(UberContext context, ILogger<Delivery> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Delivery entity)
        {
            if (entity == null)
            {
                logger.LogWarning(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes  ");
            }
            await context.Deliveries.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Delivery Added Successfully ! ");

        }

        public async Task Delete(int id)
        {
            var ISFound = await context.Deliveries.FindAsync(id);
            if (ISFound == null)
            {
                logger.LogWarning($" Delivery With ID {id} Not Found , Try Again ");
                throw new NotFoundException($" Delivery With ID {id} Not Found , Try Again ");
            }
            context.Deliveries.Remove(ISFound);
            await SaveChange();
            logger.LogInformation(" Delivery Deleted Successfully ! ");


        }

        public async Task<List<Delivery>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Deliveries.Include(A=>A.trip).Include(a=>a.Driver).ThenInclude(a=>a.user)
             .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Delivery> GetByID(int ID)
        {
            var isfound = await context.Deliveries.Include(a=>a.Driver).ThenInclude(a=>a.user).Include(a=>a.trip).FirstOrDefaultAsync(Id=>Id.Id == ID);
            if (isfound == null)
            {
                logger.LogWarning($" Delivery With ID {ID} Not Found , Try Again ");
                throw new NotFoundException($" Delivery With ID {ID} Not Found , Try Again ");
            }
            return isfound;
        }


        public async Task<Delivery> Update(int ID, Delivery entity)
        {
            var isfound = await context.Deliveries.FindAsync(ID);
            if (isfound == null)
            {
                logger.LogWarning($" Delivery With ID {ID} Not Found , Try Again ");
                throw new NotFoundException($" Delivery With ID {ID} Not Found , Try Again ");
            }
            isfound.DriverID = entity.DriverID;
            isfound.DropoffLat = entity.DropoffLat;
            isfound.DropoffLng = entity.DropoffLng;
            isfound.TripId = entity.TripId;
            isfound.Status = entity.Status;
            isfound.PickupLat = entity.PickupLat;
            isfound.PickupLng = entity.PickupLng;

            await SaveChange();
            logger.LogInformation(" Delivery Updated Successfully ! ");
            return isfound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();

        }
    }
}
