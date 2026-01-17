using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using ProposalService.Application.Interfaces;
using ProposalService.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ProposalService.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus
{
    private readonly IConfiguration _configuration;
    private const string ExchangeName = "proposalservice";
    private const string RoutingKey = "ac";

    public RabbitMqEventBus(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishProposalApprovedAsync(Proposal proposal)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMq:Host"] ?? "localhost",
            UserName = _configuration["RabbitMq:Username"] ?? "guest",
            Password = _configuration["RabbitMq:Password"] ?? "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();

        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true
        );

        var message = JsonSerializer.Serialize(new
        {
            Id = proposal.Id,
            CustomerName = proposal.CustomerName,
            InsuredAmount = proposal.InsuredAmount,
            Status = proposal.Status,
            ApprovedAt = proposal.UpdatedAt
        });

        var body = Encoding.UTF8.GetBytes(message);

        var props = new BasicProperties();

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body);

        Console.WriteLine($"[RabbitMQ] Notification event sent to '{ExchangeName}' using RK '{RoutingKey}'");
    }
}