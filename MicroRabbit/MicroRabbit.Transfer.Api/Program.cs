using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Api.Extensions;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using Microsoft.EntityFrameworkCore;
namespace MicroRabbit.Transfer.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<TransferDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("TransferDbConnection")));

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//mediatr
			builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
			builder.RegisterServices();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			ConfigureEventBus(app);

			app.Run();
		}

		private static void ConfigureEventBus(WebApplication app)
		{
			var eventBus = app.Services.GetRequiredService<IEventBus>();
			eventBus.SubscribeAsync<TransferCreatedEvent, TransferEventHandler>();
		}
	}
}
