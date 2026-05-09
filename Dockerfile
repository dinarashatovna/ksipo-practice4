# syntax=docker/dockerfile:1
# Двухстадийная сборка (multi-stage): 1) компиляция в SDK-образе, 2) запуск в лёгком runtime.
# Итоговый образ меньше, т.к. в финальную стадию не попадают SDK и исходники.

# ========== Стадия build: .NET SDK, restore + publish ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
# Режим сборки по заданию (можно переопределить: docker build --build-arg Configuration=Debug)
ARG Configuration=Release
WORKDIR /src

# Сначала только .csproj — слой с NuGet кэшируется при мелких правках в коде
COPY Practice4.csproj .
RUN dotnet restore Practice4.csproj

COPY . .
# Публикация в /app/publish; UseAppHost=false — запуск через dotnet *.dll (без нативного exe-хоста)
RUN dotnet publish Practice4.csproj \
    -c "$Configuration" \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# ========== Стадия final: только ASP.NET runtime (Alpine), без SDK ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
# wget — для healthcheck в docker-compose (HTTP-запрос к /health)
RUN apk add --no-cache wget

# Слушаем порт внутри контейнера (см. EXPOSE и ports в compose)
ENV ASPNETCORE_URLS=http://+:8080

# В образ попадает только вывод publish, не весь репозиторий
COPY --from=build /app/publish .

# Документация порта; снаружи в compose пробрасывается 8007:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Practice4.dll"]
