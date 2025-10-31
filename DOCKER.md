# 🐳 Docker инструкции

Этот документ описывает, как запустить Telegram AI Companion в Docker контейнере.

## Предварительные требования

- Docker Desktop установлен ([Скачать](https://www.docker.com/products/docker-desktop))
- Ollama запущен на хосте (не в Docker)
- Telegram Bot Token

## Быстрый старт с Docker Compose

### 1. Настройте переменные окружения

Создайте файл `.env` из примера:

```bash
cp .env.example .env
```

Отредактируйте `.env`:

```env
TELEGRAM_BOT_TOKEN=1234567890:ABCdefGHIjklMNOpqrsTUVwxyz
TELEGRAM_BOT_USERNAME=my_ai_companion_bot
```

### 2. Убедитесь, что Ollama запущен

```bash
# На вашей хост-машине
ollama list
ollama pull deepseek-r1:8b
```

### 3. Запустите бота

```bash
docker-compose up -d
```

### 4. Проверьте логи

```bash
docker-compose logs -f
```

Вы должны увидеть:
```
[INF] Starting Telegram Companion Bot
[INF] Ollama service is available
[INF] Starting bot @my_ai_companion_bot
[INF] Bot started successfully
```

### 5. Остановите бота

```bash
docker-compose down
```

## Ручной запуск с Docker

### Сборка образа

```bash
docker build -t tg-companion:latest .
```

### Запуск контейнера

```bash
docker run -d \
  --name tg-companion-bot \
  --restart unless-stopped \
  -e Telegram__BotToken="YOUR_BOT_TOKEN" \
  -e Telegram__BotUsername="your_bot_username" \
  -e Ollama__BaseUrl="http://host.docker.internal:11434" \
  -v $(pwd)/logs:/app/logs \
  --add-host=host.docker.internal:host-gateway \
  tg-companion:latest
```

### Проверка логов

```bash
docker logs -f tg-companion-bot
```

### Остановка и удаление

```bash
docker stop tg-companion-bot
docker rm tg-companion-bot
```

## Важные заметки

### Доступ к Ollama с Docker

Бот в Docker контейнере подключается к Ollama на хосте через `host.docker.internal`.

**Linux пользователи:** 

Docker Compose файл уже настроен с `host-gateway`. Если используете ручной запуск:

```bash
docker run ... --add-host=host.docker.internal:host-gateway ...
```

**macOS/Windows:**

`host.docker.internal` работает из коробки.

### Переменные окружения

Все настройки из `appsettings.json` можно переопределить через переменные окружения:

```bash
# Формат: Section__Property
-e Telegram__BotToken="token"
-e Ollama__Model="deepseek-r1:1.5b"
-e Ollama__Temperature="0.8"
-e Logging__LogLevel__Default="Debug"
```

### Логи

Логи сохраняются в volume `./logs` и доступны на хосте.

```bash
# Просмотр логов
tail -f logs/tg-companion-*.log
```

## Обновление

### С Docker Compose

```bash
# Остановите контейнер
docker-compose down

# Обновите код
git pull

# Пересоберите и запустите
docker-compose up -d --build
```

### Ручной способ

```bash
# Остановите и удалите контейнер
docker stop tg-companion-bot
docker rm tg-companion-bot

# Пересоберите образ
docker build -t tg-companion:latest .

# Запустите новый контейнер
docker run ...
```

## Решение проблем

### Ошибка: "Ollama service is not available"

**Проблема:** Контейнер не может подключиться к Ollama на хосте.

**Решение для Linux:**
```bash
# Убедитесь, что Ollama слушает на всех интерфейсах
export OLLAMA_HOST=0.0.0.0:11434
ollama serve
```

**Решение для macOS/Windows:**
- Убедитесь, что Ollama запущен
- Проверьте, что `host.docker.internal` резолвится:
  ```bash
  docker run --rm alpine ping host.docker.internal
  ```

### Ошибка: "Failed to load configuration"

**Проблема:** Неправильный формат переменных окружения.

**Решение:** Используйте `__` (двойное подчеркивание) для иерархии:
```bash
-e Telegram__BotToken="..."  # ✓ Правильно
-e Telegram:BotToken="..."   # ✗ Неправильно
```

### Контейнер постоянно перезапускается

```bash
# Проверьте логи
docker logs tg-companion-bot

# Запустите в интерактивном режиме для отладки
docker run --rm -it \
  -e Telegram__BotToken="..." \
  tg-companion:latest
```

## Production deployment

Для production рекомендуется:

1. Использовать secrets для токенов
2. Настроить log rotation
3. Использовать health checks
4. Настроить мониторинг (Prometheus, Grafana)

Пример с Docker secrets:

```yaml
version: '3.8'

services:
  tg-companion:
    # ... other config
    secrets:
      - telegram_bot_token
    environment:
      - Telegram__BotToken=/run/secrets/telegram_bot_token

secrets:
  telegram_bot_token:
    file: ./secrets/bot_token.txt
```

---

**Готово! Бот работает в Docker! 🐳✨**

