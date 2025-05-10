using Microsoft.EntityFrameworkCore;
using HomenagensApp.Models;

namespace HomenagensApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Homenagem> Homenagens => Set<Homenagem>();
    }
}