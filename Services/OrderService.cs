using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.OrderDTO;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber
{
    public class OrderService : IOrderService
    {
        private readonly UberContext context;
        private readonly IOrderRepo orderRepo;
        private readonly ILogger<Order> logger;
        private readonly IMapper mapper;

        public OrderService(UberContext context, IOrderRepo orderRepo, ILogger<Order> logger, IMapper mapper)
        {
            this.context = context;
            this.orderRepo = orderRepo;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task<CreateOrUpdateOrder> CreateOrder(CreateOrUpdateOrder Create)
        {
            if (Create == null) { throw new ArgumentNullException(" Please Enter The Feildes  "); }

            var customer = await context.Customers.Include(A=>A.UserApp)
               .FirstOrDefaultAsync(u => u.UserApp.Email == Create.CustomerEmail);

            if (customer == null)
                throw new NotFoundException($" Customer With Email Not Found : {Create.CustomerEmail}");
            var item = await context.Items.Include(a=>a.merchant).FirstOrDefaultAsync(i => i.Name == Create.ItemName );
            if (item == null)
                throw new NotFoundException($"Item {Create.ItemName} not found for this merchant.");
            var categoryExists = await context.Categories
                .AnyAsync(c => c.Id == item.CategoryId);

            if (!categoryExists)
                throw new NotFoundException($"Category Not Found {Create.ItemName}.");

            if(Create.TotalAmount > item.Quantity)
            {
                logger.LogError($" Quantity Of [{item.Name}] is {item.Quantity}  , I am Sorry ");
                throw new BadRequestException($" Quantity Of [{item.Name}] is {item.Quantity}  , I am Sorry ");
            }


            var IsMapped = mapper.Map<Order>(Create); 
            IsMapped.ItemID = item.Id;
            IsMapped.MerchantID = item.merchant.Id;
            IsMapped.CustometID = customer.Id;
            await orderRepo.Create(IsMapped);
            logger.LogInformation("  Order Created Successfully   !");
            item.Quantity -= Create.TotalAmount;
            context.Items.Update(item);
            await context.SaveChangesAsync();
            return mapper.Map<CreateOrUpdateOrder>(IsMapped);
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var IsFound = await context.Orders.FindAsync(id);
            if (IsFound == null)
            {
                logger.LogError($" Order With Id {id} Not Found ");
                throw new NotFoundException($" Order With Id {id} Not Found ");
            }
            await orderRepo.Delete(id);
            logger.LogInformation(" Deleted Successfully ");
            return true;
        }

        public async Task<List<OrderListDTO>> GetAllOrdersAsync()
        {
            var List = await orderRepo.FindAll();
            return mapper.Map<List<OrderListDTO>>(List);

        }

        public async Task<int> GetCustomerOrderCountAsync(string customerEmail)
        {
            return await context.Orders.Include(a => a.user)
                 .Include(A => A.item)
                 .Include(a => a.merchant).ThenInclude(a=>a.UserApp).
                 Where(a => a.user.UserApp.Email == customerEmail).CountAsync();

        }

        public async Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId)
        {
            var order =  await orderRepo.GetByID(orderId);

            if (order == null)
            {
                logger.LogError($"Order with Id {orderId} not found.");
                throw new NotFoundException($"Order with Id {orderId} not found.");
            }

            return mapper.Map<OrderDetailsDTO>(order);
        }

        public async Task<List<OrderByCustomerDTO>> GetOrdersByCustomerEmailAsync(string customerEmail)
        {
            var IsFound = await orderRepo.GetOrdersByCustomerEmailAsync(customerEmail);
            if (IsFound == null || !IsFound.Any())
            {
                logger.LogError($" Customer With Email {customerEmail} Not Found ");
                throw new NotFoundException($" Customer With Email {customerEmail} Not Found ");
            }
            return mapper.Map<List<OrderByCustomerDTO>>(IsFound);


        }

        public async Task<List<OrderListDTO>> GetOrdersByMerchantEmailAsync(string merchantEmail)
        {
            var IsFound = await orderRepo.GetOrdersByMerchantEmailAsync(merchantEmail);
            if (IsFound == null || !IsFound.Any())
            {
                logger.LogError($"  merchant With Email {merchantEmail} Not Found ");
                throw new NotFoundException($"  merchant With Email {merchantEmail} Not Found ");
            }
            return mapper.Map<List<OrderListDTO>>(IsFound);
        }

        public async Task<List<OrderListDTO>> SearchOrdersAsync(SearchOrderDTO search)
        {
            var query = context.Orders
                .Include(o => o.user).ThenInclude(a=>a.UserApp)
                .Include(o => o.merchant).ThenInclude(a=>a.UserApp)
                .Include(o => o.item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search.CustomerEmail))
                query = query.Where(o => o.user.UserApp.Email == search.CustomerEmail);

            if (!string.IsNullOrEmpty(search.MerchantEmail))
                query = query.Include(A=>A.user).Where(o => o.user.UserApp.Email == search.MerchantEmail);

            if (!string.IsNullOrEmpty(search.Status))
            {
                if (Enum.TryParse<OrderStatus>(search.Status, true, out var status))
                {
                    query = query.Where(o => o.Status == status);
                }
            }

            if (search.ItemId.HasValue)
                query = query.Where(o => o.item.Id == search.ItemId);


            var orders = await query.ToListAsync();

            if (orders == null || !orders.Any())
            {
                logger.LogWarning("No orders found matching the search criteria.");
                return new List<OrderListDTO>();
            }


            return mapper.Map<List<OrderListDTO>>(orders);
        }

        public async Task<CreateOrUpdateOrder> UpdateOrder(int id, CreateOrUpdateOrder update)
        {
            var existingOrder = await orderRepo.GetByID(id);

            if (existingOrder == null)
            {
                logger.LogError($"Order with Id {id} not found.");
                throw new NotFoundException($"Order with Id {id} not found.");
            }
            if (update == null) { throw new ArgumentNullException(" Please Enter The Feildes  "); }

            var item = await context.Items.FirstOrDefaultAsync(i => i.Name == update.ItemName );
            if (item == null)
                throw new NotFoundException($"Item {update.ItemName} not found for this merchant.");

            var Customer = await context.Customers.Include(a=>a.UserApp).FirstOrDefaultAsync(a=>a.UserApp.Email == update.CustomerEmail);
            if (Customer == null)
                throw new NotFoundException($"Customer With Email {update.CustomerEmail} not found for this merchant.");




            mapper.Map(update, existingOrder);

            await orderRepo.Update(id, existingOrder);

            logger.LogInformation($"Order with Id {id} updated successfully.");

            return mapper.Map<CreateOrUpdateOrder>(existingOrder);
        }
    }
}
