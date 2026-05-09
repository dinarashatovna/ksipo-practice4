# syntax=docker/dockerfile:1
# Multi-stage: сборка в SDK-образе, запуск в минимальном aspnet (Alpine).

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG Configuration=Release
WORKDIR /src

COPY Practice4.csproj .
RUN dotnet restore Practice4.csproj

COPY . .
RUN dotnet publish Practice4.csproj \
    -c "$Configuration" \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
RUN apk add --no-cache wget

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Practice4.dll"]
