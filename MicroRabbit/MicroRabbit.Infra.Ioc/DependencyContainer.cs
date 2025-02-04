using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repositoty;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Ioc
{
	public class DependencyContainer
	{
		public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			//data context
			var connectionString = configuration.GetConnectionString("BankingDbConnection");
			services.AddDbContext<BankingDbContext>(options =>
			{
				options.UseSqlServer(connectionString);
			});

			//domain bus
			services.AddTransient<IEventBus, RabbitMQBus>();

			//Repositories
			services.AddTransient<IAccountRepository, AccountRepository>();

			//application services
			services.AddTransient<IAccountService, AccountService>();
		}
	}
}
