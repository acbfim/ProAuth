using Microsoft.AspNetCore.Identity;

namespace ProAuth.WebAPI.Models.Identity
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }

    }
}