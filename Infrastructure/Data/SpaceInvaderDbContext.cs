using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Infrastructure.Data
{
    public class SpaceInvaderDbContext : DbContext
    {
        public SpaceInvaderDbContext(DbContextOptions<SpaceInvaderDbContext> options)
            : base(options)
        {
        }

        public DbSet<PlayerScore> PlayerScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}