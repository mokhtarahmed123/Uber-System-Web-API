using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.PaymentDTOs;
using Uber.Uber.Domain.Exceptions;

namespace Uber.Uber
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo paymentRepo;
        private readonly ILogger<Payment> logger;
        private readonly IMapper mapper;
        private readonly UberContext context;

        public PaymentService(IPaymentRepo paymentRepo, ILogger<Payment> logger, IMapper mapper, UberContext context)
        {
            this.paymentRepo = paymentRepo;
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CreatePaymentDTO> Create(CreatePaymentDTO request)
        {

            if (request == null)
                throw new ArgumentNullException(nameof(request), "Please Fill All Fields");

            // Find customer by email
            var customer = await context.Customers.Include(a=>a.UserApp)
                .FirstOrDefaultAsync(u => u.UserApp.Email == request.CustomerEmail);

            if (customer == null)
            {
                logger.LogError($"Customer with Email {request.CustomerEmail} not found");
                throw new NotFoundException($"Customer with Email {request.CustomerEmail} not found");
            }

            // Find trip by ID
            var trip = await context.Trips.FindAsync(request.TripID);
            if (trip == null)
            {
                logger.LogError($"Trip with ID {request.TripID} not found");
                throw new NotFoundException($"Trip with ID {request.TripID} not found");
            }

            // Manual mapping from DTO to Entity
            var payment = new Payment
            {
                Method = request.PaymentMethod,
                PaymentStatus = request.PaymentStatus,
                TripID = trip.ID,
                customerid = customer.Id,
                TotalPrice = trip.TotalCost // Use trip's total cost, not from DTO

            };

            // Handle merchant if provided
            if (!string.IsNullOrEmpty(request.MerchantEmail))
            {
                var merchant = await context.Merchants
                    .Include(m => m.UserApp)
                    .FirstOrDefaultAsync(m => m.UserApp.Email == request.MerchantEmail);

                if (merchant == null)
                {
                    logger.LogError($"Merchant with Email {request.MerchantEmail} not found");
                    throw new NotFoundException($"Merchant with Email {request.MerchantEmail} not found");
                }

                payment.Merchantid = merchant.Id;
            }

            await paymentRepo.Create(payment);
            logger.LogInformation("Payment created successfully with ID: {PaymentId}", payment.ID);

            // Manual mapping back to DTO for return
            return new CreatePaymentDTO
            {
                PaymentMethod = payment.Method,
                PaymentStatus = payment.PaymentStatus,
                TotalPrice = (int)payment.TotalPrice,
                TripID = payment.TripID,
                CustomerEmail = customer.UserApp.Email,
                MerchantEmail = !string.IsNullOrEmpty(request.MerchantEmail) ? request.MerchantEmail : null
            };
        }

        public async Task<bool> Delete(int id)
        {

            if (id <= 0)
            {
                logger.LogError(" Id Must Be Greater Than 0 ");
                throw new BadRequestException(" Id Must Be Greater Than 0 ");
            }

            var PaymentISFound = await paymentRepo.GetByID(id);
            if (PaymentISFound == null)
            {
                logger.LogError($" Payment With ID  {id} Not Found , Try Again  ");
                throw new NotFoundException($" Payment With ID {id} Not Found , Try Again  ");
            }

            await paymentRepo.Delete(id);
            logger.LogInformation("Payment Deleted successfully.");

            return true;
        }

        public async Task<List<PaymentListDTO>> GetAllAsync()
        {
            var payments = await paymentRepo.FindAll();

            // Manual mapping from Entity to PaymentListDTO
            var paymentListDTOs = new List<PaymentListDTO>();

            foreach (var payment in payments)
            {
                paymentListDTOs.Add(new PaymentListDTO
                {
                    ID = payment.ID,
                    TotalPrice = payment.TotalPrice,
                    Method = payment.Method,
                    PaymentStatus = payment.PaymentStatus,
                    UserEmail = payment.customer.UserApp?.Email ?? string.Empty,
                    MerchantName = payment.Merchant?.UserApp?.Name
                });
            }

            return paymentListDTOs;
        }

        public async Task<PaymentDetailsDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                logger.LogError(" Id Must Be Greater Than 0 ");
                throw new BadRequestException(" Id Must Be Greater Than 0 ");
            }
            var payment = await paymentRepo.GetByID(id);
            if (payment == null)
            {
                logger.LogError($"Payment with ID {id} not found");
                throw new NotFoundException($"Payment with ID {id} not found");
            }

            // Manual mapping from Entity to PaymentDetailsDTO
            return new PaymentDetailsDTO
            {
                TotalPrice = payment.TotalPrice,
                Method = payment.Method,
                PaymentStatus = payment.PaymentStatus,
                CustomerId = payment.customer.Id,
                UserName = payment.customer.UserApp?.Name ?? string.Empty,
                MerchantId = payment.Merchantid ?? 0,
                MerchantName = payment.Merchant?.UserApp?.Name ?? string.Empty,
                TripID = payment.TripID
            };
        }

        public async Task<List<PaymentDetailsDTO>> GetPaymentDetailByCustomerEmail(string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new BadRequestException("Please enter the email");

            // Check if customer exists
            var customer = await context.Users
                .FirstOrDefaultAsync(u => u.Email == customerEmail);

            if (customer == null)
                throw new NotFoundException($"Customer with Email {customerEmail} not found");

            // Get payments for the customer
            var payments = await context.Payments
                .Include(p => p.Trip_Id)
                .Include(p => p.Merchant)
                    .ThenInclude(m => m.UserApp)
                .Include(p => p.customer)
                .Where(p => p.customer.UserApp.Email == customerEmail)
                .ToListAsync();

            // Manual mapping to PaymentDetailsDTO list
            var paymentDetailsList = new List<PaymentDetailsDTO>();

            foreach (var payment in payments)
            {
                paymentDetailsList.Add(new PaymentDetailsDTO
                {
                    TotalPrice = payment.TotalPrice,
                    Method = payment.Method,
                    PaymentStatus = payment.PaymentStatus,
                    CustomerId = payment.customer.Id,
                    UserName = payment.customer.UserApp?.Name ?? string.Empty,
                    MerchantId = payment.Merchantid ?? 0,
                    MerchantName = payment.Merchant?.UserApp?.Name ?? string.Empty,
                    TripID = payment.TripID
                });
            }

            return paymentDetailsList;


        }

        public async Task<List<PaymentDetailsDTO>> GetPaymentDetailByMerchantEmail(string merchantEmail)
        {
            if (string.IsNullOrWhiteSpace(merchantEmail))
                throw new BadRequestException("Please enter the email");

            // Check if merchant exists
            var merchant = await context.Merchants
                .Include(m => m.UserApp)
                .FirstOrDefaultAsync(m => m.UserApp.Email == merchantEmail);

            if (merchant == null)
                throw new NotFoundException($"Merchant with Email {merchantEmail} not found");

            // Get payments for the merchant
            var payments = await context.Payments
                .Include(p => p.Trip_Id)
                .Include(p => p.Merchant)
                    .ThenInclude(m => m.UserApp)
                .Include(p => p.customer)
                .Where(p => p.Merchant.UserApp.Email == merchantEmail)
                .ToListAsync();

            // Manual mapping to PaymentDetailsDTO list
            var paymentDetailsList = new List<PaymentDetailsDTO>();

            foreach (var payment in payments)
            {
                paymentDetailsList.Add(new PaymentDetailsDTO
                {
                    TotalPrice = payment.TotalPrice,
                    Method = payment.Method,
                    PaymentStatus = payment.PaymentStatus,
                    CustomerId = payment.customerid,
                    UserName = payment.customer.UserApp?.Name ?? string.Empty,
                    MerchantId = payment.Merchantid ?? 0,
                    MerchantName = payment.Merchant?.UserApp?.Name ?? string.Empty,
                    TripID = payment.TripID
                });
            }

            return paymentDetailsList;

        }

        public async Task<List<PaymentListDTO>> SearchPaymentsAsync(SearchPaymentDTO search)
        {


            if (search == null)
                throw new ArgumentNullException(nameof(search), "Search request cannot be null");

            var query = context.Payments
                .Include(p => p.customer)
                .Include(p => p.Merchant)
                    .ThenInclude(m => m.UserApp)
                .Include(p => p.Trip_Id)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(search.CustomerEmail))
                query = query.Where(p => p.customer.UserApp .Email == search.CustomerEmail);

            if (search.PaymentStatus.HasValue)
                query = query.Where(p => p.PaymentStatus == search.PaymentStatus.Value);

            if (search.Method.HasValue)
                query = query.Where(p => p.Method == search.Method.Value);

            var payments = await query.ToListAsync();

            if (!payments.Any())
            {
                logger.LogWarning("No payments found matching the search criteria");
                return new List<PaymentListDTO>();
            }

            // Manual mapping to PaymentListDTO list
            var paymentListDTOs = new List<PaymentListDTO>();

            foreach (var payment in payments)
            {
                paymentListDTOs.Add(new PaymentListDTO
                {
                    ID = payment.ID,
                    TotalPrice = payment.TotalPrice,
                    Method = payment.Method,
                    PaymentStatus = payment.PaymentStatus,
                    UserEmail = payment.customer.UserApp?.Email ?? string.Empty,
                    MerchantName = payment.Merchant?.UserApp?.Name
                });
            }

            return paymentListDTOs;
        }

        public async Task<UpdatePaymentDTO> Update(int id, UpdatePaymentDTO request)
        {
            if (id <= 0)
            {
                logger.LogError(" Id Must Be Greater Than 0 ");
                throw new BadRequestException(" Id Must Be Greater Than 0 ");
            }
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Please fill all fields");

            // Get existing payment
            var existingPayment = await paymentRepo.GetByID(id);
            if (existingPayment == null)
                throw new NotFoundException($"Payment with ID {id} not found");

            // Manual mapping from UpdatePaymentDTO to existing Payment entity
            existingPayment.Method = request.PaymentMethod;
            existingPayment.PaymentStatus = request.PaymentStatus;

            // Update in repository
            await paymentRepo.Update(id, existingPayment);
            logger.LogInformation("Payment with ID {PaymentId} updated successfully", id);

            // Manual mapping back to UpdatePaymentDTO for return
            return new UpdatePaymentDTO
            {
                PaymentMethod = existingPayment.Method,
                PaymentStatus = existingPayment.PaymentStatus
            };

        }
    }
}
