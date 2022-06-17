using MicroRabbit.Baking.Application.Interfaces;
using MicroRabbit.Baking.Application.Services;
using MicroRabbit.Baking.Data.Context;
using MicroRabbit.Baking.Data.Repositories;
using MicroRabbit.Baking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroRabbit.Infra.IoC
{
    public class DependecyContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain Bus
            services.AddTransient<IEventBus, RabbitMQBus>();
            services.Configure<RabbitMQSettings>(c => configuration.GetSection("RabbitMQSettings"));

            // Application services
            services.AddTransient<IAccountService, AccountService>();

            // Data
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<BakingDbContext>();
        }
    }
}
