
using Microsoft.EntityFrameworkCore;

using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application
{
    public class PaymentRepo : IPaymentRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Payment> logger;

        public PaymentRepo(UberContext context, ILogger<Payment> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task Create(Payment entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.Payments.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Payment Added Successfully ");
        }

        public async Task Delete(int id)
        {
            var isfound = await context.Payments.FindAsync(id);
            if (isfound != null)
            {
                context.Payments.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" Payment Deleted Successfully ");
                return;
            }
            logger.LogError($" Payment With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Payment With ID {id} Not Found , try Again  ");
        }

        public async Task<List<Payment>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Payments.Include(a=>a.customer).Include(a=>a.Trip_Id).Include(A=>A.Merchant).ThenInclude(a=>a.UserApp)
            .Skip((page - 1) * pageSize)
        .Take(pageSize)
            .ToListAsync();
        }

        public async Task<Payment> GetByID(int id)
        {
            var isfound = await context.Payments.Include(a => a.customer).Include(a => a.Trip_Id).Include(A => A.Merchant).ThenInclude(a=>a.UserApp).FirstOrDefaultAsync(a=>a.ID == id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" Payment With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Payment With ID {id} Not Found , try Again  ");
        }


        public async Task<Payment> Update(int id, Payment entity)
        {
            var isfound = await context.Payments.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($" Payment With ID {id} Not Found , try Again  ");
                throw new NotFoundException($" Payment With ID {id} Not Found , try Again  ");
            }
            isfound.Method = entity.Method;
            isfound.PaymentStatus = entity.PaymentStatus;
            isfound.TripID = entity.TripID;
            isfound.TotalPrice = entity.TotalPrice;
            isfound.customerid = entity.customerid;
            isfound.Merchantid = entity.Merchantid;
            await SaveChange();
            logger.LogInformation(" Payment Updated Successfully ");
            return isfound;
        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }

    }
}
