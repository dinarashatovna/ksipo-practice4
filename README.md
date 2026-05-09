# Практика 4 — Docker + Docker Compose (Practice4)

ASP.NET Core Web API «Прогулка по саду» (логика из ПЗ3), контейнеризация с **multi-stage** `Dockerfile` и запуск через **Docker Compose**.

## Вариант 7 — порт

Внешний порт в формате **80xx**: **8007** → в браузере: `http://localhost:8007/api/flowers`.

Настраивается в `.env`: `HOST_PORT=8007`.

## Запуск локально (без Docker)

```bash
dotnet run
```

## Сборка и запуск в Docker Compose

Из папки `practice4`:

```bash
docker compose build
docker compose up -d
```

Проверка:

- http://localhost:8007/api/flowers  
- http://localhost:8007/api/config  
- http://localhost:8007/health  

Остановка: `docker compose down` (данные SQLite в именованном volume `flower-data` сохранятся).

## Файлы задания

| Файл | Назначение |
|------|------------|
| `Dockerfile` | Multi-stage: **build** (SDK) → **final** (aspnet Alpine, минимальный runtime) |
| `docker-compose.yml` | Порты, volumes, healthcheck, `env_file` |
| `.env` | Переменные Compose (`HOST_PORT`, `DOCKER_IMAGE_NAME`) |
| `config/appsettings.Production.json` | Внешняя конфигурация, монтируется в контейнер |
| `.dockerignore` | Исключения для контекста сборки (уменьшает образ и ускоряет build) |
| `.gitignore` | Для .NET / Visual Studio |

## Публикация образа в Docker Hub

1. Зарегистрируйся на https://hub.docker.com  
2. Войди в CLI: `docker login`  
3. Собери образ с тегом под свой логин:

```bash
docker build -t ВАШ_ЛОГИН/practice4:latest --build-arg Configuration=Release .
docker push ВАШ_ЛОГИН/practice4:latest
```

4. В `.env` можно указать `DOCKER_IMAGE_NAME=ВАШ_ЛОГИН/practice4:latest` и использовать `docker compose pull` на сервере (если образ публичный).

## Git + GitHub / GitLab

```bash
git init
git add .
git commit -m "Practice4: Docker and Compose"
```

Создай **публичный** репозиторий на GitHub/GitLab, добавь `remote` и `git push`.

Убедись, что в репозиторий не попали секреты и локальные `.db` (учтено в `.gitignore`).
