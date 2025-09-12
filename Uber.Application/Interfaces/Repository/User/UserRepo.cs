using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class UserRepo : IUserRepo
    {
        private readonly UberContext context;
        private readonly ILogger<UserRepo> logger;
        private readonly IMapper mapper;

        public UserRepo(UberContext context, ILogger<UserRepo> logger, IMapper mapper)

        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Create(User entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("User Name cannot be empty");

            await context.Users.AddAsync(entity);
            await SaveChange();
            logger.LogInformation("User {UserName} created successfully.", entity.Name);
        }

        public async Task Delete(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await SaveChange();
                logger.LogInformation("User with ID {Id} deleted successfully.", id);
                return;
            }

            logger.LogWarning("User with ID {Id} not found for deletion.", id);
            throw new NotFoundException($"User with ID {id} not found.");
        }
        public async Task<List<User>> FindAll(int page = 1, int pageSize = 20)
        {
            return await context.Users
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<User> GetByID(int ID)
        {
            var user = await context.Users.FindAsync(ID);
            if (user != null)
                return user;

            logger.LogWarning("User with ID {Id} not found.", ID);
            throw new NotFoundException($"User with ID {ID} not found.");
        }
        public async Task<User> Update(int ID, User entity)
        {
            var user = await context.Users.FindAsync(ID);
            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                    throw new ArgumentException("User Name cannot be empty");

                user.Name = entity.Name;
                user.PhoneNumber = entity.PhoneNumber;
                user.Email = entity.Email;
                user.IsActive = entity.IsActive;
                user.RoleId = entity.RoleId;

                await SaveChange();
                logger.LogInformation("User with ID {Id} updated successfully.", ID);
                return user;
            }

            logger.LogWarning("User with ID {Id} not found for update.", ID);
            throw new NotFoundException($"User with ID {ID} not found.");
        }

        public async Task SaveChange()
        {
            await context.SaveChangesAsync();
        }

    }
}
