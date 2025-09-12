using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application.DTOs.ReviewsDTOs;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber.Application.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly IReviewsRepo reviewsRepo;
        private readonly ILogger<Reviews> logger;

        public ReviewsService( UberContext context , IMapper mapper , IReviewsRepo reviewsRepo , ILogger<Reviews> logger )
        {
            this.context = context;
            this.mapper = mapper;
            this.reviewsRepo = reviewsRepo;
            this.logger = logger;
        }
        #region Create Review
        public async Task<ReviewDetailsDTO> CreateReviewAsync(CreateReviewDTO createReviewDTO)
        {

            if (createReviewDTO == null)
            {
                logger.LogError("Please provide all required fields.");
                throw new ArgumentNullException(nameof(createReviewDTO), "Please provide all required fields.");
            }

            if (string.IsNullOrWhiteSpace(createReviewDTO.CustomerUserEmail) ||
                string.IsNullOrWhiteSpace(createReviewDTO.DriverEmail))
            {
                logger.LogError("Customer email and driver email are required.");
                throw new ArgumentException("Customer email and driver email are required.");
            }

            var customer = await context.Customers
                .Include(c => c.UserApp)
                .FirstOrDefaultAsync(u => u.UserApp.Email == createReviewDTO.CustomerUserEmail);

            if (customer == null)
                throw new NotFoundException($"Customer with email {createReviewDTO.CustomerUserEmail} not found.");

            var driver = await context.DriverProfiles
                .Include(d => d.user)
                .FirstOrDefaultAsync(d => d.user.Email == createReviewDTO.DriverEmail);

            if (driver == null)
                throw new NotFoundException($"Driver with email {createReviewDTO.DriverEmail} not found.");

            var trip = await context.Trips
                .AsNoTracking() // 🟢 نتجنب مشكلة tracking
                .FirstOrDefaultAsync(t => t.ID == createReviewDTO.TripID);

            if (trip == null)
                throw new NotFoundException($"Trip with ID {createReviewDTO.TripID} not found.");

            // ✅ منع التكرار
            var existingReview = await context.Reviews
                .Include(r => r.customer).ThenInclude(c => c.UserApp)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TripID == createReviewDTO.TripID && r.customer.Id == customer.Id);

            if (existingReview != null)
            {
                logger.LogWarning($"Customer {customer.UserApp.Email} already reviewed trip {createReviewDTO.TripID}");
                throw new BadRequestException("You have already reviewed this trip.");
            }

            var mapped = mapper.Map<Reviews>(createReviewDTO);
            mapped.customerID = customer.Id;
            mapped.DriverID = driver.ID;
            mapped.TripID = trip.ID;

            try
            {
                await reviewsRepo.Create(mapped);
                logger.LogInformation($"Review created successfully for Trip {trip.ID} by Customer {customer.UserApp.Email}");

                return mapper.Map<ReviewDetailsDTO>(mapped);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create review.");
                throw new BadRequestException("Failed to create review, please try again later.");
            }
        }
        #endregion

        #region Delete Review
        public async Task<bool> DeleteReviewAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException($"Id [{id}] must be greater than 0.");

            var review = await reviewsRepo.GetByID(id);
            if (review == null)
                throw new NotFoundException($"Review with ID {id} not found.");

            try
            {
                await reviewsRepo.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to delete review with ID {id}.");
                throw new BadRequestException("Failed to delete review, please try again later.");
            }
        }
        #endregion
        #region Get Driver Reviews
        public async Task<List<ReviewDetailsDTO>> DriverReviewsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Driver email cannot be null or empty.");

            bool driverExists = await context.DriverProfiles.AnyAsync(d => d.user.Email == email);
            if (!driverExists)
                throw new NotFoundException($"Driver with Email [{email}] not found.");

            var reviews = await context.Reviews
                .Include(r => r.Driver).ThenInclude(a=>a.user)
                .Include(r => r.customer).ThenInclude(a=>a.UserApp).Include(a=>a.Trip)
                .Where(r => r.Driver.user.Email == email)
                .ToListAsync();

            return mapper.Map<List<ReviewDetailsDTO>>(reviews);
        }
        #endregion

        #region Get All Reviews
        public async Task<List<ReviewDetailsDTO>> GetAll()
        {
            var list = await reviewsRepo.FindAll();
            return mapper.Map<List<ReviewDetailsDTO>>(list);
        }
        #endregion

        #region Get Review By Id
        public async Task<ReviewDetailsDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException($"Id [{id}] must be greater than 0.");

            var review = await reviewsRepo.GetByID(id);
            if (review == null)
                throw new NotFoundException($"Review with ID [{id}] not found.");

            return mapper.Map<ReviewDetailsDTO>(review);
        }
        #endregion

        #region Get Customer Reviews
        public async Task<List<ReviewDetailsDTO>> GetCustomerReviewsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Customer email cannot be null or empty.");

            bool customerExists = await context.Users.AnyAsync(u => u.Email == email);
            if (!customerExists)
                throw new NotFoundException($"Customer with Email [{email}] not found.");

            var reviews = await context.Reviews
                .Include(r => r.customer).ThenInclude(a=>a.UserApp)
                .Include(r => r.Driver).ThenInclude(a=>a.user)
                .Include(r => r.Trip)
                .Where(r => r.customer.UserApp.Email == email)
                .ToListAsync();

            return mapper.Map<List<ReviewDetailsDTO>>(reviews);
        }
        #endregion

        #region Get Customer Reviews Count
        public async Task<int> GetCustomerReviewsCountAsync(string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new BadRequestException("Customer email cannot be null or empty.");

            bool exists = await context.Users.AnyAsync(u => u.Email == customerEmail);
            if (!exists)
                throw new NotFoundException($"Customer with Email [{customerEmail}] not found.");

            return await context.Reviews.Include(a=>a.customer).ThenInclude(a=>a.UserApp).Include(a=>a.Driver).ThenInclude(a => a.user).CountAsync(r => r.customer.UserApp.Email == customerEmail);
        }
        #endregion

        #region Get Driver Average Rating
        public async Task<double> GetDriverAverageRatingAsync(string driverEmail)
        {
            if (string.IsNullOrWhiteSpace(driverEmail))
                throw new BadRequestException("Driver email cannot be null or empty.");
             
            var IsFound = await context.Reviews.Include(a=>a.Driver).FirstOrDefaultAsync(a=>a.Driver.user.Email == driverEmail);
            if (IsFound == null)
                throw new NotFoundException($" Driver With Email [{driverEmail}] Not Found , TRY AGAIN  ");
           
            var driverReviews = await context.Reviews
                .Include(r => r.Driver)
                .Where(r => r.Driver.user.Email == driverEmail)
                .ToListAsync();

            if (driverReviews.Count == 0)
                return 0;

            return driverReviews.Average(r => (int)r.Rating);
        }
        #endregion

        #region Get Driver Reviews Count
        public async Task<int> GetDriverReviewsCountAsync(string driverEmail)
        {
            if (string.IsNullOrWhiteSpace(driverEmail))
                throw new BadRequestException("Driver email cannot be null or empty.");

            bool exists = await context.DriverProfiles.AnyAsync(d => d.user.Email == driverEmail);
            if (!exists)
                throw new NotFoundException($"Driver with Email [{driverEmail}] not found.");

            return await context.Reviews.CountAsync(r => r.Driver.user.Email == driverEmail);
        }
        #endregion



        #region Get Recent Reviews
        public async Task<List<ReviewDetailsDTO>> GetRecentReviewsAsync(int count)
        {
            if (count <= 0)
                throw new BadRequestException("Count must be greater than 0.");

            var recentReviews = await context.Reviews
                .Include(r => r.customer).ThenInclude(a=>a.UserApp)
                .Include(r => r.Driver).ThenInclude(a=>a.user)
                .Include(r => r.Trip)
                .OrderByDescending(r => r.ID)
                .Take(count)
                .ToListAsync();

            return mapper.Map<List<ReviewDetailsDTO>>(recentReviews);
        }
        #endregion
        #region Update Review
        public async Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO updateReviewDTO)
        {
            if (id <= 0)
                throw new BadRequestException($"Id [{id}] must be greater than 0.");

            if (updateReviewDTO == null)
                throw new BadRequestException("Please provide review details.");

            var existingReview = await reviewsRepo.GetByID(id);
            if (existingReview == null)
                throw new NotFoundException($"Review with ID [{id}] not found.");

            try
            {
                var updatedReview = mapper.Map(updateReviewDTO, existingReview);
                await reviewsRepo.Update(id, updatedReview);
                return mapper.Map<UpdateReviewDTO>(updatedReview);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to update review with ID {id}.");
                throw new BadRequestException($"Update failed: {ex.Message}");
            }
        }
        #endregion


    }
}
