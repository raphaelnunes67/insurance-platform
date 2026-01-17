using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProposalService.Infrastructure;

public class DatabaseBootstrap
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseBootstrap> _logger;

    public DatabaseBootstrap(IConfiguration configuration, ILogger<DatabaseBootstrap> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void Setup()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseBootstrap).Assembly)
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Erro to execute migration.");
            throw result.Error;
        }

        _logger.LogInformation("Migrations done!");
    }
}