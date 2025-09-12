using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.ComplaintsDTOs;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber
{
    public class ComplaintService : IComplaintsService
    {
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly IComplaintsRepo complaintsRepo;
        private readonly ILogger<Complaints> logger;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ComplaintService(UberContext context,
            IMapper mapper, 
            IComplaintsRepo complaintsRepo, 
            ILogger<Complaints> logger,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.mapper = mapper;
            this.complaintsRepo = complaintsRepo;
            this.logger = logger;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateComplaintsdto> CreateAsync(CreateComplaintsdto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto), "Please enter all required fields.");

            var customer = await context.Customers.Include(a=>a.UserApp).FirstOrDefaultAsync(u => u.UserApp.Email == createDto.CustomerEmail);
            if (customer == null)
                throw new NotFoundException($"Customer with email '{createDto.CustomerEmail}' was not found.");

            var driver = await context.DriverProfiles.Include(c=>c.user).FirstOrDefaultAsync(u => u.user.Email == createDto.DriverEmail);
            if (driver == null)
                throw new NotFoundException($"Driver with email '{createDto.DriverEmail}' was not found.");

            var trip = await context.Trips.FirstOrDefaultAsync(t => t.ID == createDto.TripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID {createDto.TripId} was not found.");

            var complaint = mapper.Map<Complaints>(createDto);
            complaint.FromUserID = customer.Id;
            complaint.AgainstUserId = driver.ID;
            complaint.TripID = trip.ID;
            complaint.IsResolved = false;

            await complaintsRepo.Create(complaint);

            logger.LogInformation("Complaint created successfully for Trip ID: {TripId}", createDto.TripId);

            return mapper.Map<CreateComplaintsdto>(complaint);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var IsFound = await context.Complaints.Include(A => A.Driver).Include(a => a.FromUser).Include(a => a.Trip).FirstOrDefaultAsync(a => a.Id == id);
            if (IsFound == null)
                throw new NotFoundException($" Complaint With Id {id} Not Found , Try Again ");
            await complaintsRepo.Delete(id);
            logger.LogInformation(" Complaint Deleted Successfully! ");
            return true;
        }

        public async Task<List<ListComplaintsDTO>> GetAllComplaintsAsync()
        {
            var List = await complaintsRepo.FindAll();
            return mapper.Map<List<ListComplaintsDTO>>(List);
        }

        public async Task<List<ListComplaintsDTO>> GetComplaintByCustomer()
        {

            var userId = httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new System.UnauthorizedAccessException("Invalid token");
            var UserISfound = await userManager.FindByIdAsync(userId);
            if (UserISfound == null)
                throw new NotFoundException($" User With Email {UserISfound.Id}  Not Found  ");
            
            var Comp = await context.Complaints.Include(a=>a.FromUser).Include(a=>a.Driver).Include(a=>a.Trip).Where(a=>a.FromUser.UserApp.Id == userId).ToListAsync();
            return mapper.Map<List<ListComplaintsDTO>>(Comp);
        }

        public async Task<ComplaintDetailsDTO> GetComplaintByIdAsync(int id)
        {
            var IsFound = await complaintsRepo.GetByID(id);
            if (IsFound == null)
                throw new NotFoundException($" Complaint With Id {id} Not Found , Try Again ");
            var Complaint = await complaintsRepo.GetByID(id);
            return mapper.Map<ComplaintDetailsDTO>(Complaint);
        }

        public async Task<bool> ResolveComplaintAsync(int id, ResolveComplaintDTO resolveDto)
        {
            var complaint = await context.Complaints.FindAsync(id);

            if (complaint == null)
                throw new NotFoundException($"Complaint with ID {id} not found, try again.");

            mapper.Map(resolveDto, complaint);

            complaint.IsResolved = true;

            context.Complaints.Update(complaint);

            await context.SaveChangesAsync();

            return true;

        }

        public async Task<List<ListComplaintsDTO>> SearchComplaintsAsync(SearchComplaintDTO search)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search), "Search request cannot be null.");
            var query = context.Complaints
             .Include(o => o.FromUser)
             .Include(o => o.Driver)
             .Include(o => o.Trip)
             .AsQueryable();
            if (!string.IsNullOrEmpty(search.FromUserEmail))
                query = query.Where(o => o.FromUser.UserApp.Email == search.FromUserEmail);    
            if (!string.IsNullOrEmpty(search.AgainstEmail))
                query = query.Where(o => o.Driver.user.Email == search.AgainstEmail);
            if (search.TripID.HasValue && search.TripID > 0)
                query = query.Where(o => o.TripID == search.TripID);

            if (search.IsResolved.HasValue)
                query = query.Where(o => o.IsResolved == search.IsResolved);

            var complaints = await query.ToListAsync();

            return mapper.Map<List<ListComplaintsDTO>>(complaints); 



        }

        public async Task<UpdateComplaintsdto> UpdateAsync(UpdateComplaintsdto update, int id)
        {
           
            var complaint = await context.Complaints
                .Include(a => a.Driver)
                .Include(a => a.FromUser)
                .Include(a => a.Trip)
                .FirstOrDefaultAsync(a => a.Id == id);


            if (complaint == null)
                throw new NotFoundException($"Complaint with Id {id} not found, try again.");
            if (update == null)
                throw new ArgumentNullException(nameof(update), "Please provide complaint details to update.");


            mapper.Map(update, complaint);
            var updatedComplaint = await complaintsRepo.Update(id, complaint);

            return mapper.Map<UpdateComplaintsdto>(updatedComplaint);
        }
    }
}
