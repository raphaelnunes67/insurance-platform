# Contracting Service

This microservice is built using **.NET 8** and follows the **Hexagonal Architecture** (Ports and Adapters) pattern. This structure ensures a clear separation of concerns, making the application testable and independent of external agents (like databases or UIs).

## 📂 Project Structure

The solution is divided into four main layers located inside the `src/` directory:

### 1. 🔌 ContractingService.Api (Entry Point)
This is the **Presentation Layer** and acts as a Primary Adapter. It handles incoming HTTP requests and background events.

* **`Controllers/`**: Contains API endpoints (e.g., `ContractsController.cs`) that receive requests and delegate work to the Application layer.
* **`Workers/`**: Contains background services.
    * `ProposalApprovedConsumer.cs`: A RabbitMQ consumer that listens for approved proposals to automatically generate contracts.
* **`Program.cs`**: Configures Dependency Injection (DI), database connections, and middleware.

### 2. 🧠 ContractingService.Application (Use Cases)
This layer orchestrates the application logic. It depends only on the Domain and defines interfaces for the Infrastructure.

* **`DTOs/`**: Data Transfer Objects (e.g., `SignContractDto`, `ProposalApprovedDto`) used to pass data between layers without exposing internal entities.
* **`Interfaces/`**: Defines the contracts for business services (e.g., `IContractService`).
* **`Services/`**: Implements the use cases (e.g., `ContractService.cs`), coordinating the Domain and Repository layers.

### 3. 💎 ContractingService.Domain (Core)
The heart of the software. It contains enterprise logic and is independent of frameworks, databases, or external libraries.

* **`Entities/`**: Represents the core business objects (e.g., `Contract.cs`) matching the database table structure.
* **`Interfaces/`**: Defines the Repository pattern contracts (e.g., `IContractRepository`). These are "Ports" implemented by the Infrastructure layer.

### 4. 🏗️ ContractingService.Infrastructure (Adapters)
This layer implements the interfaces defined by the Domain/Application layers, connecting to external resources.

* **`Repositories/`**: Implementation of data access logic (`ContractingRepository.cs`) using **Dapper**.
* **`Scripts/`**: SQL migration scripts (e.g., `001_CreateContractingsTable.sql`) used to evolve the database schema.
* **`DatabaseBootstrap.cs`**: Handles the execution of **DbUp** migrations at application startup.

---

## 🐳 Root Files

* **`Dockerfile`**: Instructions to build the application image for production.
* **`docker-compose.yml`**: Service-specific composition file for running the API and its dependencies isolated.
* **`ContractingService.sln`**: The .NET Solution file linking all projects together.