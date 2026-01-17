# Proposal Service

This microservice is built using **.NET 8** and follows the **Hexagonal Architecture** (Ports and Adapters) pattern. It is responsible for creating, validating, and managing insurance proposals. It acts as the **Producer** in the event-driven architecture, publishing messages to RabbitMQ when a proposal is approved.

## 📂 Project Structure

The solution is divided into four main layers inside the `src/` directory, plus a testing project:

### 1. 🔌 ProposalService.Api (Entry Point)
This is the **Presentation Layer** and acts as a Primary Adapter. It handles incoming HTTP requests.

* **`Controllers/`**: Exposes endpoints (e.g., `ProposalsController.cs`) to create and update proposals.
* **`Program.cs`**: Configures Dependency Injection (DI), including the RabbitMQ producer and Database connections.
* **`Storage/`**: Contains `InMemoryProposalRepository.cs` (likely for initial prototyping or testing purposes), though the production path uses the Infrastructure layer.

### 2. 🧠 ProposalService.Application (Use Cases)
This layer orchestrates the application logic. It depends only on the Domain and defines interfaces for the Infrastructure.

* **`DTOs/`**: Data Transfer Objects (e.g., `CreateProposalDto`, `UpdateStatusDto`) ensuring the domain entities remain isolated from the API contract.
* **`Interfaces/`**: Defines the contracts for business services and external adapters:
    * `IProposalService.cs`: Business logic contract.
    * `IEventBus.cs`: Abstraction for the message broker (RabbitMQ), allowing the application to publish events without knowing implementation details.
* **`Services/`**: Implements the use cases (`ProposalService.cs`), applying business rules and calling the Event Bus when necessary.

### 3. 💎 ProposalService.Domain (Core)
The heart of the software. It contains enterprise logic and is independent of frameworks.

* **`Entities/`**: Represents the core business objects (e.g., `Proposal.cs`).
* **`Interfaces/`**: Defines the Repository pattern contracts (e.g., `IProposalRepository.cs`).

### 4. 🏗️ ProposalService.Infrastructure (Adapters)
This layer implements the interfaces defined by the Domain/Application layers, connecting to external resources.

* **`Messaging/`**: Implementation of the Event Bus (`RabbitMqEventBus.cs`). This is responsible for publishing events to the RabbitMQ exchange.
* **`Repositories/`**: Implementation of data access logic (`ProposalRepository.cs`) using **Dapper**.
* **`Scripts/`**: SQL migration scripts (e.g., `001_CreateProposalsTable.sql`) for PostgreSQL.
* **`DatabaseBootstrap.cs`**: Handles the execution of **DbUp** migrations at application startup.

### 5. 🧪 Tests
* **`ProposalService.UnitTests/`**: Contains unit tests for the application logic and domain rules, ensuring reliability before deployment.

---

## 🐳 Root Files

* **`Dockerfile`**: Instructions to build the application image for production.
* **`docker-compose.yml`**: Service-specific composition file for running the API and its dependencies isolated.
* **`ProposalService.sln`**: The .NET Solution file linking all projects together.