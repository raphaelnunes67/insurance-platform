using System.Text;
using System.Text.Json;
using ContractingService.Application.DTOs;
using ContractingService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ContractingService.Api.Workers;

public class ProposalApprovedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;
    
    private const string ExchangeName = "proposalservice";
    
    private const string QueueName = "approvedcontract";
    
    private const string RoutingKey = "ac";

    public ProposalApprovedConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:Host"] ?? "localhost",
            UserName = _configuration["RabbitMq:Username"] ?? "guest",
            Password = _configuration["RabbitMq:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName, 
            type: ExchangeType.Direct, 
            durable: true, 
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: QueueName, 
            exchange: ExchangeName, 
            routingKey: RoutingKey, 
            cancellationToken: stoppingToken);

        Console.WriteLine($"[Worker] Aguardando propostas aprovadas na fila '{QueueName}'...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try 
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var proposalDto = JsonSerializer.Deserialize<ProposalApprovedDto>(message, options);

                if (proposalDto != null)
                {
                    Console.WriteLine($"[Worker] Proposta recebida: {proposalDto.Id} - {proposalDto.CustomerName}");
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var contractService = scope.ServiceProvider.GetRequiredService<IContractService>();
                        await contractService.CreateContractFromProposalAsync(proposalDto);
                    }
                    
                    Console.WriteLine($"[Worker] Contrato criado com sucesso para: {proposalDto.Id}");
                }
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Worker] Erro ao processar mensagem: {ex.Message}");
            }
        };

        await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}