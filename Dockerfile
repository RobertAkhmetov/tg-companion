# Telegram AI Companion Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TgCompanion.sln ./
COPY src/TgCompanion.Domain/TgCompanion.Domain.csproj ./src/TgCompanion.Domain/
COPY src/TgCompanion.Application/TgCompanion.Application.csproj ./src/TgCompanion.Application/
COPY src/TgCompanion.Infrastructure/TgCompanion.Infrastructure.csproj ./src/TgCompanion.Infrastructure/
COPY src/TgCompanion.Api/TgCompanion.Api.csproj ./src/TgCompanion.Api/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
WORKDIR /src/src/TgCompanion.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Environment variables (override these at runtime)
ENV Telegram__BotToken=""
ENV Telegram__BotUsername=""
ENV Ollama__BaseUrl="http://host.docker.internal:11434"
ENV Ollama__Model="deepseek-r1:8b"

ENTRYPOINT ["dotnet", "TgCompanion.Api.dll"]

