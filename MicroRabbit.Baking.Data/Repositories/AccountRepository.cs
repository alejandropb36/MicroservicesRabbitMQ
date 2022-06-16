using MicroRabbit.Baking.Data.Context;
using MicroRabbit.Baking.Domain.Interfaces;
using MicroRabbit.Baking.Domain.Models;

namespace MicroRabbit.Baking.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BakingDbContext _context;

        public AccountRepository(BakingDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAccounts() => _context.Accounts;
    }
}
