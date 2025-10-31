# 📊 Обзор проекта Telegram AI Companion

## 🎯 Общая информация

**Telegram AI Companion** - современный Telegram бот с интеграцией DeepSeek-R1 через Ollama.

- **Язык**: C# 13
- **Платформа**: .NET 9.0
- **Архитектура**: Clean Architecture
- **AI модель**: DeepSeek-R1 (через Ollama)
- **Telegram библиотека**: Telegram.Bot 22.0.0

## 📁 Структура проекта

```
tg-companion/
├── 📄 README.md                      # Основная документация
├── 📄 QUICKSTART.md                  # Быстрый старт за 5 минут
├── 📄 DOCKER.md                      # Docker инструкции
├── 📄 CONTRIBUTING.md                # Руководство для контрибьюторов
├── 📄 CHANGELOG.md                   # История изменений
├── 📄 LICENSE                        # MIT License
├── 📄 PROJECT_SUMMARY.md             # Этот файл
│
├── 🐳 Dockerfile                     # Docker образ
├── 🐳 docker-compose.yml             # Docker Compose конфигурация
├── ⚙️ global.json                    # .NET SDK версия
├── ⚙️ .editorconfig                  # Стиль кода
├── 🚫 .gitignore                     # Git ignore правила
│
├── 📦 TgCompanion.sln                # Solution файл
│
└── 📂 src/
    │
    ├── 📂 TgCompanion.Domain/              # 🏛️ DOMAIN LAYER
    │   ├── Entities/
    │   │   ├── ChatMessage.cs              # Сообщение в чате
    │   │   ├── Conversation.cs             # Контекст разговора
    │   │   └── MessageRole.cs              # Роль (User/Assistant/System)
    │   └── Interfaces/
    │       ├── IAiService.cs               # Интерфейс AI сервиса
    │       ├── IConversationRepository.cs  # Интерфейс репозитория
    │       └── ITelegramBotService.cs      # Интерфейс Telegram сервиса
    │
    ├── 📂 TgCompanion.Application/         # 💼 APPLICATION LAYER
    │   ├── DTOs/
    │   │   └── TelegramMessageDto.cs       # DTO для сообщений
    │   ├── UseCases/
    │   │   ├── ProcessMessageUseCase.cs    # Обработка сообщений
    │   │   └── ClearConversationUseCase.cs # Очистка истории
    │   └── DependencyInjection.cs          # DI регистрация
    │
    ├── 📂 TgCompanion.Infrastructure/      # 🔌 INFRASTRUCTURE LAYER
    │   ├── Configuration/
    │   │   ├── OllamaSettings.cs           # Настройки Ollama
    │   │   └── TelegramSettings.cs         # Настройки Telegram
    │   ├── Services/
    │   │   ├── OllamaAiService.cs          # Интеграция с Ollama
    │   │   └── TelegramBotService.cs       # Интеграция с Telegram
    │   ├── Repositories/
    │   │   └── InMemoryConversationRepository.cs  # In-memory хранилище
    │   └── DependencyInjection.cs          # DI регистрация
    │
    └── 📂 TgCompanion.Api/                 # 🚀 API/PRESENTATION LAYER
        ├── BotWorker.cs                    # Background Worker
        ├── Program.cs                      # Точка входа
        ├── appsettings.json                # Конфигурация
        ├── appsettings.Development.json    # Dev конфигурация
        └── appsettings.example.json        # Пример конфигурации
```

## 🏗️ Архитектура

### Clean Architecture слои

```
┌─────────────────────────────────────────────┐
│          API Layer (Presentation)           │
│  - Worker Service                           │
│  - Dependency Injection                     │
│  - Configuration                            │
└──────────────┬──────────────────────────────┘
               │ depends on
┌──────────────▼──────────────────────────────┐
│        Infrastructure Layer                 │
│  - Telegram Bot Service (External API)      │
│  - Ollama AI Service (External API)         │
│  - In-Memory Repository                     │
└──────────────┬──────────────────────────────┘
               │ depends on
┌──────────────▼──────────────────────────────┐
│         Application Layer                   │
│  - Use Cases (Business Logic)               │
│  - DTOs                                     │
│  - Orchestration                            │
└──────────────┬──────────────────────────────┘
               │ depends on
┌──────────────▼──────────────────────────────┐
│           Domain Layer                      │
│  - Entities (Pure Business Logic)           │
│  - Interfaces (Contracts)                   │
│  - No Dependencies!                         │
└─────────────────────────────────────────────┘
```

### Зависимости между слоями

- ✅ API → Infrastructure → Application → Domain
- ✅ Каждый слой зависит только от внутренних слоев
- ✅ Domain не имеет внешних зависимостей
- ✅ Dependency Inversion через интерфейсы

## 🎯 Основные компоненты

### Domain Layer (Ядро)

**Entities:**
- `ChatMessage` - сообщение в чате с метаданными
- `Conversation` - контекст разговора с историей
- `MessageRole` - enum роли (User/Assistant/System)

**Interfaces:**
- `IAiService` - работа с AI (генерация ответов)
- `IConversationRepository` - хранение разговоров
- `ITelegramBotService` - работа с Telegram Bot API

### Application Layer (Бизнес-логика)

**Use Cases:**
- `ProcessMessageUseCase` - основная логика обработки сообщений:
  - Получение/создание разговора
  - Добавление сообщения в историю
  - Генерация ответа от AI
  - Отправка ответа пользователю
  - Обработка ошибок

- `ClearConversationUseCase` - очистка истории:
  - Удаление разговора из репозитория
  - Отправка подтверждения

**DTOs:**
- `TelegramMessageDto` - данные входящего сообщения

### Infrastructure Layer (Интеграция)

**Ollama AI Service:**
- HTTP клиент для Ollama API
- Генерация ответов (sync и streaming)
- Конфигурация модели и параметров
- Проверка доступности сервиса

**Telegram Bot Service:**
- Получение обновлений (long polling)
- Отправка сообщений
- Обработка команд
- Индикатор "печатает..."

**In-Memory Repository:**
- Хранение разговоров в ConcurrentDictionary
- Thread-safe операции
- Быстрый доступ

### API Layer (Presentation)

**BotWorker:**
- Background Service
- Проверка доступности Ollama
- Запуск/остановка бота
- Lifecycle management

**Program.cs:**
- Host configuration
- Dependency Injection setup
- Serilog configuration
- App startup

## 🔧 Конфигурация

### Настройки в appsettings.json

```json
{
  "Telegram": {
    "BotToken": "YOUR_TOKEN",           // Telegram Bot Token
    "BotUsername": "your_bot"           // Username бота
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",  // Ollama API URL
    "Model": "deepseek-r1:8b",            // Модель для использования
    "Temperature": 0.7,                    // Креативность (0-1)
    "MaxTokens": 2000,                     // Макс. длина ответа
    "TimeoutSeconds": 120,                 // Таймаут запроса
    "SystemPrompt": "Ты - AI ассистент..."  // Инструкции для AI
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TgCompanion": "Debug"
    }
  }
}
```

## 🚀 Основные возможности

### Команды бота
- `/start` - приветствие и справка
- `/help` - список команд
- `/clear` - очистка истории разговора
- `/about` - информация о боте

### Функции
- ✅ Контекстные диалоги (история последних 10 сообщений)
- ✅ Множественные чаты одновременно
- ✅ Индикатор "печатает..." во время генерации
- ✅ Обработка ошибок с user-friendly сообщениями
- ✅ Структурированное логирование
- ✅ Graceful shutdown
- ✅ Проверка доступности Ollama при старте

## 📊 Технологический стек

### Core
- **.NET 9.0** - последняя версия .NET
- **C# 13** - современный C# с file-scoped namespaces, record types, etc.

### Libraries
- **Telegram.Bot 22.0.0** - Telegram Bot API
- **Serilog** - структурированное логирование
  - Serilog.Extensions.Hosting
  - Serilog.Settings.Configuration
  - Serilog.Sinks.Console
  - Serilog.Sinks.File
- **Microsoft.Extensions.*** - DI, Configuration, Hosting

### Patterns & Practices
- **Clean Architecture** - разделение на слои
- **SOLID Principles** - все 5 принципов
- **Dependency Injection** - через Microsoft.Extensions.DependencyInjection
- **Options Pattern** - для конфигурации
- **Repository Pattern** - абстракция хранилища
- **Use Case Pattern** - бизнес-сценарии

## 📈 Расширяемость

### Легко добавить

**Новую команду:**
1. Добавить обработку в `TelegramBotService.HandleCommandAsync()`
2. (Опционально) Создать новый Use Case
3. Зарегистрировать в DI

**Новое хранилище:**
1. Создать класс, реализующий `IConversationRepository`
2. Зарегистрировать в `Infrastructure/DependencyInjection.cs`

**Новый AI сервис:**
1. Создать класс, реализующий `IAiService`
2. Зарегистрировать в `Infrastructure/DependencyInjection.cs`

### Идеи для расширения

- 🗄️ Database storage (SQLite, PostgreSQL)
- 📊 Статистика и аналитика
- 👥 Поддержка групповых чатов
- 🖼️ Обработка изображений (multimodal)
- 🌐 Web интерфейс для управления
- 📉 Метрики и мониторинг (Prometheus)
- 🧪 Unit и Integration тесты
- 🔐 Rate limiting и защита от спама

## 🐳 Docker поддержка

### Запуск с Docker Compose

```bash
# Настройте .env файл с токеном
docker-compose up -d

# Просмотр логов
docker-compose logs -f

# Остановка
docker-compose down
```

### Особенности

- Образ на базе .NET 9.0
- Multi-stage build (уменьшенный размер)
- Подключение к Ollama на хосте через `host.docker.internal`
- Volume для логов
- Environment variables для конфигурации

## 📚 Документация

| Файл | Описание |
|------|----------|
| **README.md** | Полная документация проекта |
| **QUICKSTART.md** | Быстрый старт за 5 минут |
| **DOCKER.md** | Docker инструкции и troubleshooting |
| **CONTRIBUTING.md** | Гайд для контрибьюторов |
| **CHANGELOG.md** | История версий и изменений |
| **PROJECT_SUMMARY.md** | Этот файл - обзор проекта |

## 🧪 Тестирование

### Как запустить

```bash
# Сборка
dotnet build

# Запуск
cd src/TgCompanion.Api
dotnet run

# Проверьте логи
tail -f logs/tg-companion-*.log
```

### Что протестировать

1. ✅ Отправка `/start` команды
2. ✅ Обычное сообщение боту
3. ✅ Команда `/clear` для очистки истории
4. ✅ Последовательный диалог (проверка контекста)
5. ✅ Несколько чатов одновременно
6. ✅ Graceful shutdown (Ctrl+C)

## 📊 Статистика проекта

- **Файлов C#**: 17
- **Строк кода**: ~1500
- **Слои архитектуры**: 4
- **Зависимостей**: 8 NuGet packages
- **Документация**: 6 MD файлов
- **Docker файлов**: 2

## 🎓 Используемые принципы

### SOLID
- ✅ **S** - Single Responsibility: каждый класс имеет одну ответственность
- ✅ **O** - Open/Closed: расширяемость через интерфейсы
- ✅ **L** - Liskov Substitution: все реализации взаимозаменяемы
- ✅ **I** - Interface Segregation: узкие специализированные интерфейсы
- ✅ **D** - Dependency Inversion: зависимость от абстракций

### Clean Code
- Осмысленные имена
- Малые функции
- Обработка ошибок
- Комментарии (XML documentation)
- Форматирование (.editorconfig)

### Async/Await
- Все IO операции асинхронные
- CancellationToken поддержка
- Правильная обработка Task

## 🔍 Мониторинг и логирование

### Serilog

**Консоль:**
- Цветной вывод
- Readable формат

**Файлы:**
- Rolling по дням
- Путь: `logs/tg-companion-YYYY-MM-DD.log`
- Structured JSON (опционально)

**Уровни:**
- Debug - детальная информация
- Information - важные события
- Warning - предупреждения
- Error - ошибки с exception

## 🚦 Запуск в production

### Рекомендации

1. **Secrets Management**
   - Используйте Azure Key Vault / AWS Secrets Manager
   - НЕ храните токены в appsettings.json в production

2. **Logging**
   - Настройте централизованное логирование (ELK, Seq)
   - Log rotation
   - Мониторинг ошибок

3. **Database**
   - Замените In-Memory на PostgreSQL/SQLite
   - Настройте backup

4. **Performance**
   - Настройте connection pooling
   - Оптимизируйте размер контекста
   - Рассмотрите caching

5. **Monitoring**
   - Health checks
   - Metrics (Prometheus)
   - Alerting

## 📞 Поддержка

- 📖 Читайте документацию
- 🐛 Создавайте Issues на GitHub
- 💬 Задавайте вопросы в Discussions
- 🤝 Вносите вклад через Pull Requests

## 📄 Лицензия

MIT License - используйте свободно!

---

**Проект готов к использованию! Наслаждайтесь общением с AI! 🤖✨**

