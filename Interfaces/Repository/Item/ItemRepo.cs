using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber
{
    public class ItemRepo : IItemRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Item> logger;
        private readonly IMapper mapper;

        public ItemRepo(UberContext context, ILogger<Item> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Item entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.Items.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Items Added Successfully ");
        }

        public async Task Delete(int id)
        {
            var isfound = await context.Items.FindAsync(id);
            if (isfound != null)
            {
                context.Items.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" Items Deleted Successfully ");
                return;
            }
            logger.LogError($" Item With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Item With ID {id} Not Found , try Again  ");
        }

        public async Task<List<Item>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Items.Include(A=>A.category).Include(a=>a.merchant).ThenInclude(m => m.UserApp)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .ToListAsync();
        }

        public async Task<Item> GetByID(int id)
        {
            var isfound = await context.Items.Include(A=>A.category).Include(a=>a.merchant).ThenInclude(m => m.UserApp).FirstOrDefaultAsync(a=>a.Id == id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" Item With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Item With ID {id} Not Found , try Again  ");
        }


        public async Task<Item> Update(int id, Item entity)
        {
            var isfound = await context.Items.Include(A => A.category).Include(a => a.merchant).ThenInclude(m => m.UserApp).FirstOrDefaultAsync(a => a.Id == id);
            if (isfound == null)
            {
                logger.LogError($" Item With ID {id} Not Found , try Again  ");
                throw new NotFoundException($" Item With ID {id} Not Found , try Again  ");
            }

            isfound.CategoryId = entity.CategoryId;
            isfound.MerchantID = entity.MerchantID;
            isfound.Name = entity.Name;
            isfound.Quantity = entity.Quantity;
            isfound.Price = entity.Price;

            await SaveChange();
            logger.LogInformation(" Items Updated Successfully ");
            return isfound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Item>> GetItemsByCategory(string categoryName)
        {
            var isfound = await context.Items.Include(a => a.category).Include(a=>a.merchant).ThenInclude(a=>a.UserApp).Where(a => a.category.Name == categoryName).ToListAsync();
            if (isfound == null)
            {

                logger.LogError($" Category With Name {categoryName} Not Found , try Again  ");
                throw new NotFoundException($"  Category With Name {categoryName} Not Found , try Again  ");

            }
            return isfound;
        }

        public async Task<List<Item>> GetItemsByMerchant(string merchantEmail)
        {
            var isfound = await context.Items.Include(A=>A.category).Include(a => a.merchant).ThenInclude(m => m.UserApp).Where(a => a.merchant.UserApp.Email == merchantEmail).ToListAsync();
            if (isfound == null)
            {

                logger.LogError($" merchant Email With Email {merchantEmail} Not Found , try Again  ");
                throw new NotFoundException($"  merchant Email With Email {merchantEmail} Not Found , try Again  ");

            }
            return isfound;
        }
    }
}
