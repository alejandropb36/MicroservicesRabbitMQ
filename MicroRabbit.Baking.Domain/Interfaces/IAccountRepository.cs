using MicroRabbit.Baking.Domain.Models;

namespace MicroRabbit.Baking.Domain.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAccounts();

    }
}
