using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Domain.Core.Bus
{
	public interface IEventBus
	{
		Task SendCommandAsync<T>(T command) where T : Command;
		Task PublishAsync<T>(T @event) where T : Event;
		Task SubscribeAsync<T, TH>() where T : Event where TH : IEventHandler<T>;
	}
}
