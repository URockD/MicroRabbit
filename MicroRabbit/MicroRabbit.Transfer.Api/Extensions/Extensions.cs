
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Ioc;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;

namespace MicroRabbit.Transfer.Api.Extensions
{
    public static class Extensions
    {
        public static void RegisterServices(this IHostApplicationBuilder builder)
        {
            DependencyContainer.RegisterServices(builder.Services);
        }

		public static IApplicationBuilder UseEventBus(this IApplicationBuilder app)
		{
			var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
			eventBus.SubscribeAsync<TransferCreatedEvent, TransferEventHandler>();
			return app;
		}
	}
}
