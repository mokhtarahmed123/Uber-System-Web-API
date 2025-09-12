
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber
{
    public class ComplaintsRepo : IComplaintsRepo
    {
        private readonly UberContext context;
        private readonly ILogger<Complaints> logger;
        private readonly IMapper mapper;

        public ComplaintsRepo(UberContext context, ILogger<Complaints> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(Complaints entity)
        {
            if (entity == null)
                throw new BadRequestException(" Please Enter All OF Data");
            await context.Complaints.AddAsync(entity);
            await SaveChange();
            logger.LogInformation("Complaint Added Successfully !!");
        }

        public async Task Delete(int id)
        {
            var ISFound = await context.Complaints.FindAsync(id);
            if (ISFound == null)
            {

                logger.LogWarning("Complaint with ID {Id} not found for deletion.", id);
                throw new NotFoundException("This Complaint Not Found , Please Try Again");

            }
            context.Complaints.Remove(ISFound);
            await SaveChange();
            logger.LogInformation("User with ID {Id} deleted successfully.", id);
            return;
        }

        public async Task<List<Complaints>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Complaints.Include(a=>a.FromUser).Include(a=>a.Driver)
            .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();
        }

        public async Task<Complaints> GetByID(int ID)
        {
            var ISFOUND = await context.Complaints.Include(A => A.Driver).Include(a => a.FromUser).Include(a => a.Trip).FirstOrDefaultAsync(a => a.Id == ID);
            if (ISFOUND == null)
            {
                logger.LogWarning($"THis {ID} Not Found");
                throw new NotFoundException(" THis {ID} Not Found");
            }
            return ISFOUND;
        }


        public async Task<Complaints> Update(int ID, Complaints entity)
        {
            var ISFOUND = await context.Complaints.FindAsync(ID);
            if (ISFOUND == null)
            {
                logger.LogWarning($"THis {ID} Not Found");
                throw new NotFoundException(" THis {ID} Not Found");
            }
            ISFOUND.AgainstUserId = entity.AgainstUserId;
            ISFOUND.FromUserID = entity.FromUserID;
            ISFOUND.IsResolved = entity.IsResolved;
            ISFOUND.TripID = entity.TripID;
            ISFOUND.Message = entity.Message;

            await SaveChange();
            logger.LogInformation($"Complaint with ID {ID} updated successfully.");
            return ISFOUND;

        }
        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }
    }
}
