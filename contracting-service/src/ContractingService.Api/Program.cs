using System.Data;
using Npgsql;
using ContractingService.Application.Interfaces;
using ContractingService.Application.Services; 
using ContractingService.Infrastructure;      
using ContractingService.Infrastructure.Repositories; 
using ContractingService.Domain.Interfaces;
using ContractingService.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ProposalApprovedConsumer>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

builder.Services.AddScoped<IContractRepository, ContractingRepository>();

builder.Services.AddScoped<IContractService, ContractingService.Application.Services.ContractService>();

builder.Services.AddSingleton<DatabaseBootstrap>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var dbBooststrap = serviceProvider.GetRequiredService<DatabaseBootstrap>();
        dbBooststrap.Setup();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fail to start database: {ex.Message}");
    }
}

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();