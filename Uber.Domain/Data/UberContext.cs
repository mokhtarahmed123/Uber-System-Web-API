using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber
{
    public class UberContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DriverProfile> DriverProfiles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Customer> Customers { get; set; }






        public UberContext()
        {

        }
        public UberContext(DbContextOptions<UberContext> options) : base(options)
        {

        }


    }

}
