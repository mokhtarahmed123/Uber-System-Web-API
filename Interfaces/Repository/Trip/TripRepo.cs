
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace Uber.Uber
{
    public class TripRepo : ITripRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Trip> logger;
        private readonly IMapper mapper;

        public TripRepo(UberContext context, ILogger<Trip> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Trip entity)
        {
            if (entity == null)
                throw new ArgumentException("Data  cannot be empty");
            await context.Trips.AddAsync(entity);
            await SaveChange();
            logger.LogInformation("Trip Added Successfully !!!!");
        }

        public async Task Delete(int id)
        {
            var IsFound = await context.Trips.FindAsync(id);
            if (IsFound == null)
                throw new NotFoundException("Trip Not Found , Try Again");
            context.Trips.Remove(IsFound);
            await SaveChange();
            logger.LogInformation($"Deleted {IsFound} Successfully !!!!");

        }

        public async Task<List<Trip>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Trips.Include(a=>a.customer).ThenInclude(a=>a.UserApp).Include(a=>a.order).Include(A => A.Driver).ThenInclude(a=>a.user)
           .Skip((page - 1) * pageSize)
          .Take(pageSize)
           .ToListAsync();
        }

        public async Task<Trip> GetByID(int ID)
        {
            var Trip = await context.Trips.Include(a=>a.customer).ThenInclude(a=>a.UserApp).Include(a=>a.order).Include(A=>A.Driver).ThenInclude(a=>a.user).FirstOrDefaultAsync(I=>I.ID == ID);
            if (Trip != null)
                return Trip;

            logger.LogWarning("Trip with ID {Id} not found.", ID);
            throw new NotFoundException($"Trip with ID {ID} not found.");
        }
        public async Task<Trip> Update(int ID, Trip entity)
        {
            var IsFound = await context.Trips.FindAsync(ID);
            if (IsFound == null)
                throw new NotFoundException("Trip Not Found , Try Again");
            IsFound.Driver = entity.Driver;
            IsFound.StartTime = entity.StartTime;
            IsFound.EndTime = entity.EndTime;
            IsFound.StatausTrip = entity.StatausTrip;
            IsFound.DurationMin = entity.DurationMin;
            IsFound.DistanceKm = entity.DistanceKm;
            IsFound.TotalCost = entity.TotalCost;
            IsFound.CarImagePath = entity.CarImagePath;
            IsFound.DriverId = entity.DriverId;
            IsFound.customerId = entity.customerId;
            IsFound.OrderID = entity.OrderID;

            await SaveChange();
            logger.LogInformation($"User with ID {ID} updated successfully.");
            return IsFound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }
    }
}
