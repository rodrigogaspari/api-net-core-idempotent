using FluentAssertions.Common;
using MediatR;
using ApiIdempotent.Application.Abstractions;
using ApiIdempotent.Infrastructure.Database;
using ApiIdempotent.Infrastructure.Database.Repository;
using ApiIdempotent.Infrastructure.Sqlite;
using System.Reflection;
using IdempotentAPI.Extensions.DependencyInjection;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        // sqlite
        builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue("DatabaseName", "Data Source=database.sqlite")});
        builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();


        builder.Services.AddScoped<DbSession>();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<SaldoRepository>();
        builder.Services.AddTransient<MovimentoRepository>();
        builder.Services.AddTransient<ContaCorrenteRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Idempotency with IdempotentAPI, learn more about IdempotentAPI at
        // https://github.com/ikyriak/IdempotentAPI/blob/master/README.md 
        builder.Services.AddIdempotentAPI();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddIdempotentAPIUsingDistributedCache();

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

        // sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        app.Run();
    }
}


// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html


