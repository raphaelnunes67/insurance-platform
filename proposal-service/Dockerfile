# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj e restaurar dependências
# Ajuste os caminhos se sua pasta src tiver outro nome
COPY src/ProposalService.Domain/*.csproj ./src/ProposalService.Domain/
COPY src/ProposalService.Application/*.csproj ./src/ProposalService.Application/
COPY src/ProposalService.Infrastructure/*.csproj ./src/ProposalService.Infrastructure/
COPY src/ProposalService.Api/*.csproj ./src/ProposalService.Api/

RUN dotnet restore ./src/ProposalService.Api/ProposalService.Api.csproj

# Copiar todo o resto e buildar
COPY src/ ./src/
WORKDIR /app/src/ProposalService.Api
RUN dotnet publish -c Release -o /app/out

# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProposalService.Api.dll"]