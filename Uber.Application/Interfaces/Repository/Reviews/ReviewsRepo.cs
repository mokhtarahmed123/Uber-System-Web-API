using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber
{
    public class ReviewsRepo : IReviewsRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Reviews> logger;

        public ReviewsRepo(UberContext context, ILogger<Reviews> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task Create(Reviews entity)
        {
            if (entity == null)
            {
                logger.LogError(" Please Enter All Fieldes ");
                throw new BadRequestException(" Please Enter All Fieldes ");
            }
            await context.Reviews.AddAsync(entity);
            await SaveChange();
            logger.LogInformation(" Review Added Successfully ");
        }

        public async Task Delete(int id)
        {
            var isfound = await context.Reviews.FindAsync(id);
            if (isfound != null)
            {
                context.Reviews.Remove(isfound);
                await SaveChange();
                logger.LogInformation(" Reviews Deleted Successfully ");
                return;
            }
            logger.LogError($" Review With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Review With ID {id} Not Found , try Again  ");
        }

        public async Task<List<Reviews>> FindAll(int page = 1, int pageSize = 20)
        { return await context.Reviews.Include(A=>A.customer).ThenInclude(a=>a.UserApp).Include(a=>a.Driver).ThenInclude(a=>a.user).Include(A=>A.Trip).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(); }

        public async Task<Reviews> GetByID(int id)
        {
            var isfound = await context.Reviews.Include(A => A.customer).ThenInclude(a=>a.UserApp).Include(a => a.Driver).ThenInclude(a => a.user).Include(A => A.Trip).FirstOrDefaultAsync(a=>a.ID == id);
            if (isfound != null)
            { return isfound; }
            logger.LogError($" Review With ID {id} Not Found , try Again  ");
            throw new NotFoundException($" Review With ID {id} Not Found , try Again  ");
        }


        public async Task<Reviews> Update(int id, Reviews entity)
        {
            var isfound = await context.Reviews.FindAsync(id);
            if (isfound == null)
            {
                logger.LogError($" Item With ID {id} Not Found , try Again  ");
                throw new NotFoundException($" Item With ID {id} Not Found , try Again  ");
            }
            isfound.Rating = entity.Rating;
            isfound.customerID = entity.customerID;
            isfound.TripID = entity.TripID;
            isfound.DriverID = entity.DriverID;
            isfound.Massege = entity.Massege;

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
