
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app


COPY src/ContractingService.Domain/*.csproj ./src/ContractingService.Domain/
COPY src/ContractingService.Application/*.csproj ./src/ContractingService.Application/
COPY src/ContractingService.Infrastructure/*.csproj ./src/ContractingService.Infrastructure/
COPY src/ContractingService.Api/*.csproj ./src/ContractingService.Api/

RUN dotnet restore ./src/ContractingService.Api/ContractingService.Api.csproj


COPY src/ ./src/
WORKDIR /app/src/ContractingService.Api
RUN dotnet publish -c Release -o /app/out


FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ContractingService.Api.dll"]