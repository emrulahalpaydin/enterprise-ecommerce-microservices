FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Microservices.ECommerce.sln .
COPY src/ApiGateway/ApiGateway/ApiGateway.csproj src/ApiGateway/ApiGateway/
RUN dotnet restore src/ApiGateway/ApiGateway/ApiGateway.csproj
COPY . .
RUN dotnet publish src/ApiGateway/ApiGateway/ApiGateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
