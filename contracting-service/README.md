
```
contracting-service/
├── ContractingService.sln
├── global.json
├── ContractingService.Domain/        # Entidades, Exceções, Interfaces (Portas de saída)
├── ContractingService.Application/   # Use Cases, Services, DTOs
├── ContractingService.Infrastructure/# EF Core, clientes HTTP, Adapters
└── ContractingService.API/           # Controllers (Adapters de entrada)
```