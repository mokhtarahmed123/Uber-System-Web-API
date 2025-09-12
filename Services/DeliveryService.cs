using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application.DTOs.DeliveryDTOs;
using Uber.Uber.Application.DTOs.OrderDTO;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;
using Uber.Uber.Domain.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Uber.Uber.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly IDeliveryRepo deliveryRepo;
        private readonly ILogger<Delivery> logger;

        public DeliveryService(UberContext context, IMapper mapper, IDeliveryRepo deliveryRepo, ILogger<Delivery> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.deliveryRepo = deliveryRepo;
            this.logger = logger;
        }
        public async Task<CreateDeliveryDTO> CreateDeliveryDTO(CreateDeliveryDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException("Please provide delivery details.");


            var user = await context.DriverProfiles.Include(a=>a.user).FirstOrDefaultAsync(u => u.user.Email == dto.DriverEmail);
            if (user == null)
                throw new NotFoundException($"User with email '{dto.DriverEmail}' was not found.");


            var driverProfile = await context.DriverProfiles
                .Include(dp => dp.user)
                .FirstOrDefaultAsync(dp => dp.user.Email == dto.DriverEmail);

            if (driverProfile == null)
                throw new NotFoundException($"Driver with email '{dto.DriverEmail}' does not have a driver profile.");

            var TripIsFound = await context.Trips.FindAsync(dto.TripId);
            if (TripIsFound == null)
                throw new NotFoundException($"Trip with Id '{dto.TripId}' Not Found");



            var delivery = mapper.Map<Delivery>(dto);

            delivery.DriverID = driverProfile.ID;
            delivery.TripId = TripIsFound.ID;

        await deliveryRepo.Create(delivery);

            return mapper.Map<CreateDeliveryDTO>(delivery);
        }



        public async Task<bool> DeleteDelivery(int id)
        {
            if (id <= 0)
            {
                logger.LogError(" ID Must Be Greater Than 0 ");
                throw new ArgumentOutOfRangeException("ID Must Be Greater Than 0");
            }

            var IsFound = await context.Deliveries.FindAsync(id);
            if (IsFound == null)
            {
                logger.LogError($"Delivery with id {id} Not Found ");
                throw new NotFoundException($"Delivery with id {id} Not Found");
            }
            await deliveryRepo.Delete(id);
            logger.LogInformation($" Delivery Removed Successfully !! ");
            return true;
        }


        public async Task<List<ListDeliveryDTO>> GetDeliveriesByStatusAsync(DeliveryStatus status)
        {
           var List = await context.Deliveries.Include(A=>A.trip).Include(a=>a.Driver).ThenInclude(A=>A.user).Where(a=>a.Status == status).ToListAsync();
            return mapper.Map<List<ListDeliveryDTO>>(List);
        }

        public async Task<DeliveryDetailsDTO> GetDeliveryByIdAsync(int id)
        {
            if (id <= 0)
            {
                logger.LogError("ID Must Be Greater Than 0");
                throw new ArgumentOutOfRangeException("ID Must Be Greater Than 0");
            }


            var Delivery = await deliveryRepo.GetByID(id);

            if (Delivery == null)
            {
                logger.LogError($"Delivery with id {id} Not Found");
                throw new NotFoundException($"Delivery with id {id} Not Found");
            }

            return mapper.Map<DeliveryDetailsDTO>(Delivery);
        }

        public async Task<List<ListDeliveryDTO>> ListDeliveryDTO()
        {
            var List = await deliveryRepo.FindAll();
            return mapper.Map<List<ListDeliveryDTO>>(List);            
        }

        public async Task<List<ListDeliveryDTO>> SearchDeliveriesAsync(SearchDeliveryDTO searchDTO)
        {
           var query =  context.Deliveries.Include(a=>a.Driver).ThenInclude(a=>a.user).Include(a=>a.trip).AsQueryable();
            if (!string.IsNullOrEmpty(searchDTO.DriverEmail))
                query = query.Where(o => o.Driver.user.Email == searchDTO.DriverEmail);
            var Delivery = await query.ToListAsync();

            if (Delivery == null || !Delivery.Any())
            {
                logger.LogWarning("No Deliveries found matching the search criteria.");
                return new List<ListDeliveryDTO>();
            }
            return mapper.Map<List<ListDeliveryDTO>>(Delivery);
        }

        public async Task<UpdateDeliveryDTO> UpdateDeliveryDTO(int id, UpdateDeliveryDTO deliveryDTO)
        {
            if(deliveryDTO == null)
            {
                logger.LogError(" Please Enter All Fieldes");
                throw new ArgumentNullException(" Please Enter All Fieldes");
            }
            if (id <= 0)
            {
                logger.LogError(" ID Must Be Greater Than 0 ");
                throw new ArgumentOutOfRangeException("ID Must Be Greater Than 0");
            }
            var IsFound = await context.Deliveries.Include(a=>a.Driver).ThenInclude(a=>a.user).FirstOrDefaultAsync(a=>a.Driver.user.Email == deliveryDTO.DriverEmail);
            if (IsFound == null) 
            {
                logger.LogError($"Delivery with id {id} Not Found ");
                throw new NotFoundException("Delivery with id {id} Not Found");
            }
            
            mapper.Map(deliveryDTO,IsFound);
            var Result = await deliveryRepo.Update(id,IsFound);
            logger.LogInformation($" delivery with Id {id} updated successfully.");
            return mapper.Map<UpdateDeliveryDTO>(IsFound);
        }

        
    }
}
