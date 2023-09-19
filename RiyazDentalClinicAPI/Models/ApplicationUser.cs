using Microsoft.AspNetCore.Identity;

namespace RiyazDentalClinicAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
