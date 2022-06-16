using MicroRabbit.Baking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Baking.Data.Context
{
    public class BakingDbContext : DbContext
    {
        public BakingDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
