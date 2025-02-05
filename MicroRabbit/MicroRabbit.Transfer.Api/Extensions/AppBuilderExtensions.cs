using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;

namespace MicroRabbit.Transfer.Api.Extensions
{
	public static class AppBuilderExtensions
	{
		public static async Task<IApplicationBuilder> UseEventBus(this IApplicationBuilder app)
		{
			var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
			await eventBus.SubscribeAsync<TransferCreatedEvent, TransferEventHandler>();
			return app;
		}
	}
}
