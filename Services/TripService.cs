using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Uber.Uber.Application.DTOs.TripDTOs;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application.Services
{
    public class TripService : ITripService
    {
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly ITripRepo tripRepo;
        private readonly ILogger<Trip> logger;

        public TripService(UberContext context, IMapper mapper, ITripRepo tripRepo, ILogger<Trip> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.tripRepo = tripRepo;
            this.logger = logger;
        }
        public async Task<CreateTripDTO> CreateTripAsync(CreateTripDTO create)
        {
            if (create == null)
            {
                logger.LogError("ENTER All Fields");
                throw new ArgumentNullException(nameof(create), "ENTER All Fields");
            }

            var driver = await context.DriverProfiles
                .Include(a => a.user)
                .FirstOrDefaultAsync(u => u.user.Email == create.DriverEmail);

            if (driver == null)
            {
                logger.LogError($"Driver with email {create.DriverEmail} not found.");
                throw new NotFoundException($"Driver with email {create.DriverEmail} not found.");
            }


            var rider = await context.Customers
                .Include(a => a.UserApp)
                .FirstOrDefaultAsync(u => u.UserApp.Email == create.RiderEmail);

            if (rider == null)
            {
                logger.LogError($"Rider with email {create.RiderEmail} not found.");
                throw new NotFoundException($"Rider with email {create.RiderEmail} not found.");
            }


            var trip = mapper.Map<Trip>(create);


            if (create.RideRequestId.HasValue)
            {
                var rideRequest = await context.RideRequests.FindAsync(create.RideRequestId.Value);
                if (rideRequest == null)
                {
                    logger.LogError($"Ride request with ID {create.RideRequestId} not found.");
                    throw new NotFoundException($"Ride request with ID {create.RideRequestId} not found.");
                }

                trip.RideRequestId = rideRequest.ID;
            }

            if (create.OrderId.HasValue)
            {
                var order = await context.Orders.FindAsync(create.OrderId.Value);
                if (order == null)
                {
                    logger.LogError($"Order with ID {create.OrderId} not found.");
                    throw new NotFoundException($"Order with ID {create.OrderId} not found.");
                }

                trip.OrderID = order.Id;
            }

            trip.DriverId = driver.ID;
            trip.customerId = rider.Id;
            await tripRepo.Create(trip);
            logger.LogInformation("Trip added successfully.");

            return mapper.Map<CreateTripDTO>(trip);
        }

        public async Task<bool> DeleteTripAsync(int id)
        {
            var IsFound = await tripRepo.GetByID(id);
            if (IsFound == null)
            {
                logger.LogError($" Trip With ID {id} Not Found , Try Again ");
                throw new NotFoundException($" Trip With ID  {id} Not Found , Try Again ");
            }
            await tripRepo.Delete(id);
            logger.LogInformation(" Trip Deleted Successfully ! ");
            return true;
        }

        public async Task<List<TripListDTO>> GetAllTripsAsync()
        {
            var List = await tripRepo.FindAll();
            return mapper.Map<List<TripListDTO>>(List);
        }

        public async Task<TripDetailsDTO> GetTripDetailsAsync(int id)
        {
            if(id <= 0)
            {
                logger.LogInformation(" Id Must Be Greater Than 0 ");
                throw new BadRequestException(" Id Must Be Greater Than 0 ");
            }
            var Trip = await tripRepo.GetByID(id);
           return mapper.Map<TripDetailsDTO>(Trip);
        }

        public async Task<List<TripListDTO>> GetTripsByDriverAsync(string driverEmail)
        {
            var Driver = await context.Trips.Include(x=>x.Driver).FirstOrDefaultAsync(a=>a.Driver.user.Email == driverEmail);
            if(Driver == null)
            {
                logger.LogError($" Driver  With Email {driverEmail} Not Found , Try Again ");
                throw new NotFoundException($" Driver  With Email {driverEmail} Not Found , Try Again ");
               
            }

            var IsFOUND = await context.Trips.Include(A=>A.Driver).Include(a=>a.customer).ThenInclude(A=>A.UserApp).Include(A=>A.RideRequest).Where(a=>a.Driver.user.Email == driverEmail).ToListAsync();
            return mapper.Map<List<TripListDTO>>(IsFOUND);
        }

        public async Task<List<SearchTripDTO>> SearchTripDTO(SearchTripDTO search)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search), "Search request cannot be null.");

            var query = context.Trips
                .Include(o => o.customer).ThenInclude(a=>a.UserApp)
                .Include(o => o.Driver)
                    .ThenInclude(d => d.user) 
                .Include(o => o.RideRequest)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.DriverEmail))
                query = query.Where(o => o.Driver.user.Email == search.DriverEmail);

            if (!string.IsNullOrWhiteSpace(search.RiderEmail))
                query = query.Where(o => o.customer.UserApp.Email == search.RiderEmail);

            if (search.StatausTrip.HasValue)
                query = query.Where(o => o.StatausTrip == search.StatausTrip.Value);

            var trips = await query.ToListAsync();

            if (!trips.Any())
            {
                logger.LogWarning("No Trips found matching the search criteria.");
                return new List<SearchTripDTO>();
            }

            return mapper.Map<List<SearchTripDTO>>(trips);

        }

        public async Task<UpdateTripDTO> UpdateTripAsync(int id, UpdateTripDTO update)
        {
            if (id <= 0)
                throw new BadRequestException("ID must be greater than 0.");

            var trip = await tripRepo.GetByID(id);
            if (trip == null)
                throw new NotFoundException($"Trip with ID {id} not found.");

            mapper.Map(update, trip);
            var updated = await tripRepo.Update(id, trip);

            logger.LogInformation($"Trip with ID {id} updated successfully.");
            return mapper.Map<UpdateTripDTO>(updated);

        }

        public async Task<UpdateTripStatusDTO> UpdateTripStatusAsync(int id, UpdateTripStatusDTO update)
        {
            if (id <= 0)
                throw new BadRequestException("ID must be greater than 0.");

            var trip = await tripRepo.GetByID(id);
            if (trip == null)
                throw new NotFoundException($"Trip with ID {id} not found.");

            trip.StatausTrip = update.StatausTrip;
            await tripRepo.Update(id, trip);

            logger.LogInformation($"Trip status with ID {id} updated successfully.");
            return update;
        }
    }
}
