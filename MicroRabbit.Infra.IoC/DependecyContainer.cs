using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repositories;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroRabbit.Infra.IoC
{
    public class DependecyContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // MediaTR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Domain Bus
            services.AddTransient<IEventBus, RabbitMQBus>();
            services.Configure<RabbitMQSettings>(c => configuration.GetSection("RabbitMQSettings"));

            // Application services
            services.AddTransient<IAccountService, AccountService>();

            // Data
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<BankingDbContext>();
        }
    }
}
