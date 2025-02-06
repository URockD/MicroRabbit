using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Bus
{
	public sealed class RabbitMQBus : IEventBus
	{
		private readonly IMediator _mediator;
		private readonly Dictionary<string, List<Type>> _handlers;
		private readonly List<Type> _eventTypes;
		private	readonly IServiceScopeFactory _serviceScopeFactory;

		public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
		{
			_mediator = mediator;
			_serviceScopeFactory = serviceScopeFactory;
			_handlers = new Dictionary<string, List<Type>>();
			_eventTypes = new List<Type>();
		}
		public async Task PublishAsync<T>(T @event) where T : Event
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			var eventName = @event.GetType().Name;
			await channel.QueueDeclareAsync(eventName, false, false, false, null);
			var message = JsonConvert.SerializeObject(@event);
			var body = Encoding.UTF8.GetBytes(message);
			await channel.BasicPublishAsync(
				exchange: string.Empty,
				routingKey: eventName,
				body: body);
		}

		public Task SendCommand<T>(T command) where T : Command
		{
			return _mediator.Send(command);
		}

		public async Task SubscribeAsync<T, TH>()
			where T : Event
			where TH : IEventHandler<T>
		{
			var eventName = typeof(T).Name;
			var handlerType = typeof(TH);
			if (!_eventTypes.Contains(typeof(T)))
			{
				_eventTypes.Add(typeof(T));
			}
			if (!_handlers.TryGetValue(eventName, out List<Type>? value))
			{
				value = new List<Type>();
				_handlers.Add(eventName, value);
			}
			if (value.Any(s => s.GetType() == handlerType))
			{
				throw new ArgumentException(
					$"Handler Type {handlerType.Name} already is registered for '{eventName}'",
					nameof(handlerType));
			}

			value.Add(handlerType);
			await StartBasicConsumeAsync<T>();
		}

		private async Task StartBasicConsumeAsync<T>() where T : Event
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			var eventName = typeof(T).Name;
			await channel.QueueDeclareAsync(eventName, false, false, false, null);
			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += ConsumerReceivedAsync;
			await channel.BasicConsumeAsync(eventName, true, consumer);
		}

		private async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs @event)
		{
			var eventName = @event.RoutingKey;
			var message = Encoding.UTF8.GetString(@event.Body.ToArray());
			try
			{
				await ProcessEventAsync(eventName, message);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private async Task ProcessEventAsync(string eventName, string message)
		{
			if (_handlers.TryGetValue(eventName, out List<Type>? handlers))
			{
				using var scope = _serviceScopeFactory.CreateScope();
				foreach (var subscription in handlers)
				{
					var handler = scope.ServiceProvider.GetRequiredService(subscription);
					if (handler == null) continue;
					var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
					if (eventType == null) continue;
					var @event = JsonConvert.DeserializeObject(message, eventType);
					if (@event == null) continue;
					var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
					var method = concreteType.GetMethod("Handle");
					if (method != null)
					{
						await _mediator.Publish(@event);
						await (Task)method.Invoke(handler, [@event])!;
					}
				}
			}
		}
	}
}
