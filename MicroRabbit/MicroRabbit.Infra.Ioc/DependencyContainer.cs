using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repositoty;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace MicroRabbit.Infra.Ioc
{
	public class DependencyContainer
	{
		public static void RegisterServices(IServiceCollection services)
		{
			//domain bus
			services.AddSingleton<IEventBus, RabbitMQBus>(static sp =>
			{
				var mediator = sp.GetService<IMediator>() ?? throw new Exception("Mediator not found");
				var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
				return new RabbitMQBus(mediator, scopeFactory);
			});

			//suscriptions
			services.AddTransient<TransferEventHandler>();

			//domain events
			services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();

			//domain banking cmds
			services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

			//application services
			services.AddTransient<IAccountService, AccountService>();
			services.AddTransient<ITransferService, TransferService>();

			//repositories
			services.AddTransient<IAccountRepository, AccountRepository>();
			services.AddTransient<ITransferRepository, TransferRepository>();

			//db context
			services.AddDbContext<BankingDbContext>();
			services.AddDbContext<TransferDbContext>();
		}
	}
}
