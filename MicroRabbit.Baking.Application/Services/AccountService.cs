using MicroRabbit.Baking.Application.Interfaces;
using MicroRabbit.Baking.Domain.Interfaces;
using MicroRabbit.Baking.Domain.Models;

namespace MicroRabbit.Baking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _repository.GetAccounts();
        }
    }
}
