using Microsoft.EntityFrameworkCore;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        { 
        }

        public DbSet<Platform> Platforms { get; set; }
    }
}
