using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application.DTOs.RideRequestDTOs;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities.Enums;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Application.DTOs.TripDTOs;
namespace Uber.Uber.Application.Services
{
    public class RideRequestsService : IRideRequestsService
    {
        private readonly IRideRequestRepo requestRepo;
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly ILogger<RideRequest> logger;

        public RideRequestsService(IRideRequestRepo requestRepo , UberContext context , IMapper mapper , ILogger<RideRequest> logger)
        {
            this.requestRepo = requestRepo;
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> AcceptRideRequest(int id)
        {
            if (id <= 0)
            {
                logger.LogError($" Id => {id} Must Be Greater Than 0  ");
                throw new BadHttpRequestException($" Id => {id} Must Be Greater Than 0  ");
            }
            var IsFound = await requestRepo.GetByID(id);
            if (IsFound == null) {
                logger.LogError($" Request With Id {id} Not Found  ");
                throw new NotFoundException($" Request With Id {id} Not Found ");
            }
            IsFound.RideRequestStatus = RideRequestsStatus.Accepted;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelRideRequest(int id) // From Rider
        {
            if (id <= 0)
            {
                logger.LogError($" Id => {id} Must Be Greater Than 0  ");
                throw new BadHttpRequestException($" Id => {id} Must Be Greater Than 0  ");
            }
            var IsFound = await requestRepo.GetByID(id);
            if (IsFound == null)
            {
                logger.LogError($" Request With Id {id} Not Found  ");
                throw new NotFoundException($" Request With Id {id} Not Found ");
            }
            IsFound.RideRequestStatus = RideRequestsStatus.Rejected;
            await context.SaveChangesAsync();
            return true;
        }

        #region Create Ride Request
        public async Task<CreateRideRequestDTO> CreateRideRequest(CreateRideRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO), "Request cannot be null.");


            var rider = await context.Customers.Include(A=>A.UserApp).FirstOrDefaultAsync(u => u.UserApp.Email == requestDTO.RiderEmail);
            if (rider == null)
            {
                logger.LogError($" Rider With Email {requestDTO.RiderEmail} Not Found ");
                throw new NotFoundException($" Rider With Email {requestDTO.RiderEmail} Not Found ");
            }

            var mapped = mapper.Map<RideRequest>(requestDTO);
            mapped.RiderID = rider.Id;
            mapped.RideRequestStatus = RideRequestsStatus.Pending; 

            await requestRepo.Create(mapped);

            mapped.Rider.UserApp.Email = requestDTO.RiderEmail;
            logger.LogInformation("Ride Request Created Successfully!");
            return mapper.Map<CreateRideRequestDTO>(mapped);
        }
        #endregion
        #region Delete Ride Request
        public async Task<bool> DeleteRequest(int id)
        {
            if (id <= 0)
                throw new BadHttpRequestException("Id must be greater than 0");

            var request = await requestRepo.GetByID(id);
            if (request == null)
            {
                logger.LogError($" Ride Request With Id {id} Not Found ");
                throw new NotFoundException($" Ride Request With Id {id} Not Found ");
            }

            await requestRepo.Delete(id);
            logger.LogInformation("Ride Request Deleted Successfully!");
            return true;
        }
        #endregion

        public async Task<List<RideRequestListDTO>> GetAcceptedRequests()
        {

            var List = await context.RideRequests.Include(a => a.Rider).ThenInclude(a => a.UserApp).Where(a => a.RideRequestStatus == RideRequestsStatus.Accepted).ToListAsync();
            return mapper.Map<List<RideRequestListDTO>>(List);
        }

        public async Task<List<RideRequestListDTO>> GetCompletedRequests()
        {

            var List = await context.RideRequests.Include(a => a.Rider).ThenInclude(a=>a.UserApp).Where(a => a.RideRequestStatus == RideRequestsStatus.Completed).ToListAsync();
            return mapper.Map<List<RideRequestListDTO>>(List);
        }

        public async Task<List<RideRequestListDTO>> GetPendingRequests()
        {

            var List = await context.RideRequests.Include(a => a.Rider).ThenInclude(a=>a.UserApp).Where(a => a.RideRequestStatus == RideRequestsStatus.Pending).ToListAsync();
            return mapper.Map<List<RideRequestListDTO>>(List);
        }

        public async Task<List<RideRequestListDTO>> ListRideRequests()
        {
            var List = await context.RideRequests.Include(a => a.Rider).ThenInclude(a => a.UserApp).ToListAsync();
            return mapper.Map<List<RideRequestListDTO>>(List);
        }

        public async Task<bool> RejectRideRequest(int id) // From Admin OR Driver
        {
            if (id <= 0)
            {
                logger.LogError($" Id => {id} Must Be Greater Than 0  ");
                throw new BadHttpRequestException($" Id => {id} Must Be Greater Than 0  ");
            }
            var IsFound = await requestRepo.GetByID(id);
            if (IsFound == null)
            {
                logger.LogError($" Request With Id {id} Not Found  ");
                throw new NotFoundException($" Request With Id {id} Not Found ");
            }
            IsFound.RideRequestStatus = RideRequestsStatus.Rejected;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<RideRequestDetailsDTO> RideRequestDetails(int id)
        {
            if (id <= 0)
            {
                logger.LogError($" Id => {id} Must Be Greater Than 0  ");
                throw new BadHttpRequestException($" Id => {id} Must Be Greater Than 0  ");
            }

            var IsFound = await requestRepo.GetByID(id);
            if (IsFound == null)
            {
                logger.LogError($" Request With Id {id} Not Found  ");
                throw new NotFoundException($" Request With Id {id} Not Found ");
            }

          return mapper.Map<RideRequestDetailsDTO>(IsFound); 

        }

        public async Task<List<RideRequestListDTO>> SearchRideRequests(SearchRideRequestDTO search)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search), "Search request cannot be null.");

            var query = context.RideRequests
                               .Include(o => o.Rider).ThenInclude(a => a.UserApp)
                               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.RiderEmail))
                query = query.Where(o => o.Rider.UserApp.Email == search.RiderEmail);


            if (search.Status != null)
            {
                if (Enum.TryParse<StatausTrip>(search.Status.ToString(), true, out var statusEnum))
                {
                    query = query.Where(o => o.RideRequestStatus == (RideRequestsStatus)statusEnum);
                }
                else
                {
                    logger.LogWarning($"Invalid status value provided: {search.Status}");
                    return new List<RideRequestListDTO>();
                }
            }


            var requests = await query.ToListAsync();

            if (!requests.Any())
            {
                logger.LogWarning("No ride requests found matching the search criteria.");
                return new List<RideRequestListDTO>();
            }

            logger.LogInformation($"Found {requests.Count} ride requests matching the criteria.");

            return mapper.Map<List<RideRequestListDTO>>(requests);
        }

        #region Update Ride Request
        public async Task<UpdateRideRequestDTO> UpdateRideRequest(int id, UpdateRideRequestDTO requestDTO)
        {
            if (id <= 0)
                throw new BadHttpRequestException($"Invalid Id {id}");
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO), "Request cannot be null.");

            var existingRequest = await requestRepo.GetByID(id);
            if (existingRequest == null)
            {
                logger.LogError($"Ride Request With Id {id} Not Found");
                throw new NotFoundException($"Ride Request With Id {id} Not Found");
            }

            if (!string.IsNullOrEmpty(requestDTO.RiderEmail))
            {
                var rider = await context.Customers.Include(a => a.UserApp).FirstOrDefaultAsync(u => u.UserApp.Email == requestDTO.RiderEmail);
                if (rider == null)
                    throw new NotFoundException($"Rider with email {requestDTO.RiderEmail} not found.");

                existingRequest.RiderID = rider.Id;
            }

            mapper.Map(requestDTO, existingRequest);

            var updated = await requestRepo.Update(id, existingRequest);

            logger.LogInformation("Ride Request Updated Successfully");
            return mapper.Map<UpdateRideRequestDTO>(updated);
        }
        #endregion
    }
}
