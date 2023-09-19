using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RiyazDentalClinicAPI.Models;

namespace RiyazDentalClinicAPI.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DoctorsTestimonial> DoctorsTestimonials { get; set; }
        public DbSet<PatientsTestimonial> PatientsTestimonials { get; set; }
        public DbSet<MyCase> MyCases { get; set; }
    }
}
