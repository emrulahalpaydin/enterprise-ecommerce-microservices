FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Microservices.ECommerce.sln .
COPY src/OrderService/OrderService.Api/OrderService.Api.csproj src/OrderService/OrderService.Api/
COPY src/OrderService/OrderService.Application/OrderService.Application.csproj src/OrderService/OrderService.Application/
COPY src/OrderService/OrderService.Domain/OrderService.Domain.csproj src/OrderService/OrderService.Domain/
COPY src/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj src/OrderService/OrderService.Infrastructure/
COPY shared/Contracts/Microservices.Shared.Contracts.csproj shared/Contracts/
COPY shared/EventBus/Microservices.Shared.EventBus.csproj shared/EventBus/
COPY shared/BuildingBlocks/Microservices.Shared.BuildingBlocks.csproj shared/BuildingBlocks/
RUN dotnet restore src/OrderService/OrderService.Api/OrderService.Api.csproj
COPY . .
RUN dotnet publish src/OrderService/OrderService.Api/OrderService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]
