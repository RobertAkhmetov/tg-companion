# Changelog

Все значимые изменения проекта документируются в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/),
и проект следует [Semantic Versioning](https://semver.org/lang/ru/).

## [Unreleased]

### Планируется
- Unit и integration тесты
- Персистентное хранилище (SQLite/PostgreSQL)
- Поддержка групповых чатов
- Web интерфейс для мониторинга
- Rate limiting
- Статистика использования

## [1.0.0] - 2025-10-31

### Добавлено
- ✨ Базовая функциональность Telegram бота
- ✨ Интеграция с Ollama DeepSeek-R1
- ✨ Clean Architecture структура проекта
- ✨ Команды: `/start`, `/help`, `/clear`, `/about`
- ✨ Контекст разговора (history)
- ✨ In-memory хранилище разговоров
- ✨ Логирование с Serilog
- ✨ Настройка через appsettings.json
- ✨ User Secrets поддержка
- 📦 Docker поддержка
- 📦 Docker Compose конфигурация
- 📖 Подробная документация (README, QUICKSTART, DOCKER)
- 📖 Примеры конфигурации
- 🏗️ .editorconfig для единого стиля кода
- 🏗️ global.json для версии SDK

### Слои архитектуры
- **Domain** - сущности и интерфейсы
  - `ChatMessage`, `Conversation`, `MessageRole`
  - `IAiService`, `IConversationRepository`, `ITelegramBotService`
  
- **Application** - бизнес-логика
  - `ProcessMessageUseCase` - обработка сообщений
  - `ClearConversationUseCase` - очистка истории
  - `TelegramMessageDto`
  
- **Infrastructure** - внешние интеграции
  - `OllamaAiService` - интеграция с Ollama
  - `TelegramBotService` - интеграция с Telegram Bot API
  - `InMemoryConversationRepository` - хранилище в памяти
  
- **Api** - точка входа
  - `BotWorker` - Background Service
  - Dependency Injection конфигурация
  - Serilog настройка

### Технологии
- .NET 9.0
- C# 13 с новейшими возможностями
- Telegram.Bot 22.0.0
- Ollama API
- Serilog для логирования
- Clean Architecture паттерн

### Документация
- README.md - основная документация
- QUICKSTART.md - быстрый старт за 5 минут
- DOCKER.md - инструкции по Docker
- CONTRIBUTING.md - руководство для контрибьюторов
- CHANGELOG.md - история изменений

## Типы изменений

- `Added` - новая функциональность
- `Changed` - изменения в существующей функциональности
- `Deprecated` - функции, которые скоро будут удалены
- `Removed` - удаленные функции
- `Fixed` - исправления багов
- `Security` - исправления безопасности

---

[Unreleased]: https://github.com/yourusername/tg-companion/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/yourusername/tg-companion/releases/tag/v1.0.0

