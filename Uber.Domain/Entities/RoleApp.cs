using Microsoft.AspNetCore.Identity;

namespace Uber.Uber.Domain.Entities
{
    public class RoleApp : IdentityRole
    {
        public List<User> Users { get; set; } = new List<User>(); public ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    }
}
