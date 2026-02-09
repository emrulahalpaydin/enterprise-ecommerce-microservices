FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Microservices.ECommerce.sln .
COPY src/CatalogService/CatalogService.Api/CatalogService.Api.csproj src/CatalogService/CatalogService.Api/
COPY src/CatalogService/CatalogService.Application/CatalogService.Application.csproj src/CatalogService/CatalogService.Application/
COPY src/CatalogService/CatalogService.Domain/CatalogService.Domain.csproj src/CatalogService/CatalogService.Domain/
COPY src/CatalogService/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj src/CatalogService/CatalogService.Infrastructure/
COPY shared/Contracts/Microservices.Shared.Contracts.csproj shared/Contracts/
COPY shared/EventBus/Microservices.Shared.EventBus.csproj shared/EventBus/
COPY shared/BuildingBlocks/Microservices.Shared.BuildingBlocks.csproj shared/BuildingBlocks/
RUN dotnet restore src/CatalogService/CatalogService.Api/CatalogService.Api.csproj
COPY . .
RUN dotnet publish src/CatalogService/CatalogService.Api/CatalogService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.Api.dll"]
