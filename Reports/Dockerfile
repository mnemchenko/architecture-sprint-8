FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

COPY ["src/Handlers/Handler.WebApi/Handler.WebApi.csproj", "src/Handlers/Handler.WebApi/"]
#COPY ["src/Application/Application.csproj", "src/Application/"]
#COPY ["src/Domain/Domain.csproj", "src/Domain/"]
#COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
#COPY ["src/Service/Service.csproj", "src/Service/"]

RUN dotnet restore "src/Handlers/Handler.WebApi/Handler.WebApi.csproj"

COPY . .

RUN dotnet build "src/Handlers/Handler.WebApi/Handler.WebApi.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "/src/Handlers/Handler.WebApi/Handler.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

#COPY ["appsettings.Shared.json", "/app/publish/appsettings.Shared.json"]

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Handler.WebApi.dll"]
