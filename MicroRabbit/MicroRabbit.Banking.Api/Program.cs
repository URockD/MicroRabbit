using MicroRabbit.Banking.Api.Extensions;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Infra.Ioc;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Banking.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("BankingDbConnection");
			builder.Services.AddDbContext<BankingDbContext>(options =>
			{
				options.UseSqlServer(connectionString);
			});

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
			app.Run();
		}
	}
}

