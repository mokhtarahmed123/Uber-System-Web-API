using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application
{
    public class MerchantRepo : IMerchantRepo
    {
        private readonly UberContext context;
        private readonly ILogger<UserRepo> logger;
        private readonly IMapper mapper;

        public MerchantRepo(UberContext context, ILogger<UserRepo> logger, IMapper mapper)

        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Merchant entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.Merchants.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Merchant Added Successfully ");
        }

        public async Task Delete(int id)
        {
            var isfound = await context.Merchants.FindAsync(id);
            if (isfound != null)
            {
                context.Merchants.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" Merchants Deleted Successfully ");
                return;
            }
            logger.LogError($" Merchants With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Merchants With ID {id} Not Found , try Again  ");
        }

        public async Task<List<Merchant>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Merchants
          .Skip((page - 1) * pageSize)
              .Take(pageSize).ToListAsync();

        }

        public async Task<Merchant> GetByID(int id)
        {
            var isfound = await context.Merchants.FindAsync(id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" Merchants With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Merchants With ID {id} Not Found , try Again  ");

        }


        public async Task<Merchant> Update(int id, Merchant entity)
        {
            var isfound = await context.Merchants.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($" Merchant With ID {id} Not Found , try Again  ");
                throw new NotFoundException($" Merchant With ID {id} Not Found , try Again  ");
            }
            //mapper.Map(entity, isfound);
            isfound.UserApp.Name = entity.UserApp.Name;
            isfound.Latitude = entity.Latitude;
            isfound.Longitude = entity.Longitude;
            isfound.UserApp.Email = entity.UserApp.Email;
            isfound.Address = entity.Address;
            isfound.AppUserId = entity.AppUserId;
            ///////////////////Edit  Data in User 
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
