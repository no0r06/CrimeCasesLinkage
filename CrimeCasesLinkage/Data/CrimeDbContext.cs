using Microsoft.EntityFrameworkCore;
using CrimeCasesLinkage.Models;

namespace CrimeCasesLinkage.Data
{
    public class CrimeDbContext : DbContext
    {
        public CrimeDbContext(DbContextOptions<CrimeDbContext> options)
            : base(options)
        {
        }

        public DbSet<CrimeCase> CrimeCases { get; set; }
        public DbSet<Victim> Victims { get; set; }
    }
}