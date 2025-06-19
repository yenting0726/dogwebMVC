using Microsoft.EntityFrameworkCore;

namespace dogwebMVC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Productdogweb> Productsbydogweb { get; set; }
       


    }







}
