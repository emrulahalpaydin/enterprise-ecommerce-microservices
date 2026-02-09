FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Microservices.ECommerce.sln .
COPY src/PaymentService/PaymentService.Api/PaymentService.Api.csproj src/PaymentService/PaymentService.Api/
COPY src/PaymentService/PaymentService.Application/PaymentService.Application.csproj src/PaymentService/PaymentService.Application/
COPY src/PaymentService/PaymentService.Domain/PaymentService.Domain.csproj src/PaymentService/PaymentService.Domain/
COPY src/PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj src/PaymentService/PaymentService.Infrastructure/
COPY shared/Contracts/Microservices.Shared.Contracts.csproj shared/Contracts/
COPY shared/EventBus/Microservices.Shared.EventBus.csproj shared/EventBus/
COPY shared/BuildingBlocks/Microservices.Shared.BuildingBlocks.csproj shared/BuildingBlocks/
RUN dotnet restore src/PaymentService/PaymentService.Api/PaymentService.Api.csproj
COPY . .
RUN dotnet publish src/PaymentService/PaymentService.Api/PaymentService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]
