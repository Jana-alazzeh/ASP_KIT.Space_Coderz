using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace July_Team.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } 
        public DbSet<Order> Orders { get; set; }     
        public DbSet<Course> Courses { get; set; }   
        public DbSet<Task> Tasks { get; set; }

        public DbSet<ContactUsViewModel> ContactUs { get; set; }


    }
}