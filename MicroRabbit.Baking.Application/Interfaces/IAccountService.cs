using MicroRabbit.Baking.Domain.Models;

namespace MicroRabbit.Baking.Application.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccounts();
    }
}
