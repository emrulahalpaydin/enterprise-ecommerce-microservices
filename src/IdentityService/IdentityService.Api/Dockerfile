FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Microservices.ECommerce.sln .
COPY src/IdentityService/IdentityService.Api/IdentityService.Api.csproj src/IdentityService/IdentityService.Api/
COPY src/IdentityService/IdentityService.Application/IdentityService.Application.csproj src/IdentityService/IdentityService.Application/
COPY src/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj src/IdentityService/IdentityService.Domain/
COPY src/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj src/IdentityService/IdentityService.Infrastructure/
COPY shared/Contracts/Microservices.Shared.Contracts.csproj shared/Contracts/
COPY shared/EventBus/Microservices.Shared.EventBus.csproj shared/EventBus/
COPY shared/BuildingBlocks/Microservices.Shared.BuildingBlocks.csproj shared/BuildingBlocks/
RUN dotnet restore src/IdentityService/IdentityService.Api/IdentityService.Api.csproj
COPY . .
RUN dotnet publish src/IdentityService/IdentityService.Api/IdentityService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]
