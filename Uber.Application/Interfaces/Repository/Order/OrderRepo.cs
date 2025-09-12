using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application
{
    public class OrderRepo : IOrderRepo
    {
        private readonly UberContext context;
        private readonly ILogger<UserRepo> logger;
        private readonly IMapper mapper;

        public OrderRepo(UberContext context, ILogger<UserRepo> logger, IMapper mapper)

        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Order entity)
        {
            if (entity == null)
            {
                logger.LogError("Please Enter All Fieldes");
                throw new BadRequestException("Please Enter All Fieldes");
            }
            await context.Orders.AddAsync(entity);
            await SaveChange();
            logger.LogInformation("Order Added Successfully");

        }

        public async Task Delete(int id)
        {
            var isfound = await context.Orders.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($"Order With ID {id} Not Found");
                throw new NotFoundException($"Order With ID {id} Not Found");
            }
            context.Orders.Remove(isfound);
            await SaveChange();
            logger.LogInformation("Order Deleted Successfully");
        }

        public async Task<List<Order>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Orders.Include(o => o.item)
             .Include(o => o.merchant).ThenInclude(m => m.UserApp)
             .Include(a=>a.user).ThenInclude(a=>a.UserApp)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<Order> GetByID(int ID)
        {
            var order = await context.Orders
        .Include(o => o.user).ThenInclude(u => u.UserApp)
        .Include(o => o.item)
        .Include(o => o.merchant).ThenInclude(m => m.UserApp)
        .FirstOrDefaultAsync(o => o.Id == ID);

            if (order == null)
            {
                logger.LogError($"Order with ID {ID} not found");
                throw new NotFoundException($"Order with ID {ID} not found");
            }

            return order;

        }

        public async Task<Order> Update(int id, Order entity)
        {
            var isfound = await context.Orders.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($"Order With ID {id} Not Found");
                throw new NotFoundException($"Order With ID {id} Not Found");
            }
            isfound.MerchantID = entity.MerchantID;
            isfound.OrderDate = entity.OrderDate;
            isfound.PaymentMethod = entity.PaymentMethod;
            isfound.Status = entity.Status;
            isfound.CustometID = entity.CustometID;
            isfound.totalAmount = entity.totalAmount;


            await SaveChange();
            logger.LogInformation("Order Updated Successfully");
            return isfound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerEmailAsync(string email)
        {
            var orders = await context.Orders
            .Include(a=>a.user).ThenInclude(A=>A.UserApp)
          .Include(o => o.merchant).ThenInclude(a=>a.UserApp)
         .Include(o => o.item)
         .Where(o => o.user.UserApp.Email == email)
           .ToListAsync();


            if (orders == null || !orders.Any())
            {
                logger.LogError($"Customer with email {email} not found or has no orders.");
                throw new NotFoundException($"Customer with email {email} not found or has no orders.");
            }

            return orders;


        }

        public async Task<List<Order>> GetOrdersByMerchantEmailAsync(string email)
        {
            var orders = await context.Orders
               .Include(o => o.user).ThenInclude(a=>a.UserApp)
                 .Include(o => o.merchant).ThenInclude (a=>a.UserApp)
           .Include(o => o.item)
            .Where(o => o.merchant.UserApp.Email == email)
           .ToListAsync();


            if (orders == null || !orders.Any())
            {
                logger.LogError($"merchant with email {email} not found or has no orders.");
                throw new NotFoundException($"merchant with email {email} not found or has no orders.");
            }

            return orders;
        }
    }
}
