# 🤖 Telegram AI Companion

Telegram бот с интеграцией **DeepSeek-R1** (через [Ollama](https://ollama.com)) для общения в чате.

Проект реализован на **.NET 9 C#** с использованием **Clean Architecture** и лучших практик разработки.

## 📋 Содержание

- [Возможности](#возможности)
- [Архитектура](#архитектура)
- [Технологии](#технологии)
- [Требования](#требования)
- [Установка и настройка](#установка-и-настройка)
- [Запуск](#запуск)
- [Использование](#использование)
- [Структура проекта](#структура-проекта)
- [Документация](#документация-1)

## ✨ Возможности

- ✅ Общение с AI-ассистентом на базе **DeepSeek-R1**
- ✅ Контекст разговора (история последних сообщений)
- ✅ Команды бота (`/start`, `/help`, `/clear`, `/about`)
- ✅ Индикатор "печатает..." во время генерации ответа
- ✅ Обработка множественных чатов одновременно
- ✅ Логирование с помощью Serilog
- ✅ Clean Architecture с разделением на слои

## 🏗️ Архитектура

Проект построен на принципах **Clean Architecture**:

```
┌─────────────────────────────────────┐
│         TgCompanion.Api             │  ← Presentation Layer
│      (Worker Service, DI)           │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    TgCompanion.Infrastructure       │  ← Infrastructure Layer
│  (Telegram Bot, Ollama, Repository) │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     TgCompanion.Application         │  ← Application Layer
│      (Use Cases, DTOs)              │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│       TgCompanion.Domain            │  ← Domain Layer
│   (Entities, Interfaces)            │
└─────────────────────────────────────┘
```

> 📖 **Подробнее об архитектуре читайте в [ARCHITECTURE.md](ARCHITECTURE.md)**

### Слои проекта

- **Domain** - чистая бизнес-логика, сущности и интерфейсы (без зависимостей)
- **Application** - use cases, бизнес-сценарии, DTOs
- **Infrastructure** - реализация внешних интеграций (Telegram Bot API, Ollama)
- **Api** - точка входа, Worker Service, конфигурация DI

## 🛠️ Технологии

- **.NET 9.0** - последняя версия .NET
- **C# 13** - современный C# с новейшими возможностями
- **Telegram.Bot** - библиотека для работы с Telegram Bot API
- **Ollama** - локальный AI-сервис с DeepSeek-R1
- **Serilog** - структурированное логирование
- **Clean Architecture** - архитектурный паттерн

## 📦 Требования

1. **.NET 9 SDK** - [Скачать](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Ollama** - [Установить](https://ollama.com/download)
3. **Telegram Bot Token** - создать через [@BotFather](https://t.me/botfather)

## 🚀 Установка и настройка

### 1. Клонирование репозитория

```bash
git clone https://github.com/yourusername/tg-companion.git
cd tg-companion
```

### 2. Установка и настройка Ollama

```bash
# Установите Ollama с официального сайта: https://ollama.com/download

# Скачайте модель DeepSeek-R1 (рекомендуется 8b версия)
ollama pull deepseek-r1:8b

# Проверьте, что модель загружена
ollama list

# Запустите Ollama (должен запуститься автоматически)
# Ollama будет доступен на http://localhost:11434
```

**Доступные модели DeepSeek-R1:**
- `deepseek-r1:1.5b` (1.1 GB) - самая легкая
- `deepseek-r1:7b` (4.7 GB) - легкая
- `deepseek-r1:8b` (5.2 GB) - **рекомендуется** ⭐
- `deepseek-r1:14b` (9.0 GB) - средняя
- `deepseek-r1:32b` (20 GB) - тяжелая
- `deepseek-r1:70b` (43 GB) - очень тяжелая
- `deepseek-r1:671b` (404 GB) - полная модель

### 3. Создание Telegram бота

1. Откройте Telegram и найдите [@BotFather](https://t.me/botfather)
2. Отправьте команду `/newbot`
3. Следуйте инструкциям для создания бота
4. Сохраните полученный **Bot Token** (например: `1234567890:ABCdefGHIjklMNOpqrsTUVwxyz`)

### 4. Настройка конфигурации

Отредактируйте файл `src/TgCompanion.Api/appsettings.json`:

```json
{
  "Telegram": {
    "BotToken": "ВАШ_BOT_TOKEN_ЗДЕСЬ",
    "BotUsername": "ваш_бот_username"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "deepseek-r1:8b",
    "Temperature": 0.7,
    "MaxTokens": 2000,
    "SystemPrompt": "Ты - дружелюбный и полезный AI-ассистент в Telegram..."
  }
}
```

**Альтернативный способ (User Secrets для разработки):**

```bash
cd src/TgCompanion.Api

# Установите токен бота
dotnet user-secrets set "Telegram:BotToken" "ВАШ_BOT_TOKEN"
dotnet user-secrets set "Telegram:BotUsername" "ваш_бот_username"
```

### 5. Восстановление зависимостей

```bash
dotnet restore
```

## ▶️ Запуск

### Запуск из командной строки

```bash
# Переход в директорию API проекта
cd src/TgCompanion.Api

# Запуск в режиме разработки
dotnet run

# Запуск с релизной конфигурацией
dotnet run --configuration Release
```

### Запуск из Visual Studio

1. Откройте `TgCompanion.sln` в Visual Studio
2. Установите `TgCompanion.Api` как стартовый проект
3. Нажмите `F5` или кнопку "Start"

### Запуск из VS Code

1. Откройте папку проекта в VS Code
2. Нажмите `F5` или выберите "Run > Start Debugging"

## 💬 Использование

### Команды бота

После запуска откройте вашего бота в Telegram и попробуйте команды:

- `/start` - приветствие и информация о боте
- `/help` - справка по командам
- `/clear` - очистить историю разговора
- `/about` - информация о боте и используемой модели

### Обычное общение

Просто напишите сообщение боту, и он ответит используя DeepSeek-R1:

```
Вы: Привет! Как дела?
Бот: Привет! У меня всё отлично, спасибо! Я AI-ассистент, 
     всегда рад помочь. Чем могу быть полезен? 😊

Вы: Расскажи анекдот про программистов
Бот: [Генерирует ответ используя DeepSeek-R1...]
```

## 📁 Структура проекта

```
tg-companion/
├── src/
│   ├── TgCompanion.Domain/              # Domain слой
│   │   ├── Entities/                    # Сущности
│   │   │   ├── ChatMessage.cs
│   │   │   ├── Conversation.cs
│   │   │   └── MessageRole.cs
│   │   └── Interfaces/                  # Интерфейсы
│   │       ├── IAiService.cs
│   │       ├── IConversationRepository.cs
│   │       └── ITelegramBotService.cs
│   │
│   ├── TgCompanion.Application/         # Application слой
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   │   └── TelegramMessageDto.cs
│   │   ├── UseCases/                    # Бизнес-сценарии
│   │   │   ├── ProcessMessageUseCase.cs
│   │   │   └── ClearConversationUseCase.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── TgCompanion.Infrastructure/      # Infrastructure слой
│   │   ├── Configuration/               # Настройки
│   │   │   ├── OllamaSettings.cs
│   │   │   └── TelegramSettings.cs
│   │   ├── Services/                    # Реализации сервисов
│   │   │   ├── OllamaAiService.cs
│   │   │   └── TelegramBotService.cs
│   │   ├── Repositories/                # Репозитории
│   │   │   └── InMemoryConversationRepository.cs
│   │   └── DependencyInjection.cs
│   │
│   └── TgCompanion.Api/                 # Presentation слой
│       ├── BotWorker.cs                 # Background Service
│       ├── Program.cs                   # Точка входа
│       ├── appsettings.json
│       └── appsettings.Development.json
│
├── TgCompanion.sln                      # Solution файл
├── .gitignore
└── README.md
```

## 🎨 Лучшие практики

Проект демонстрирует следующие практики:

### Clean Architecture
- ✅ Разделение на слои с четкими границами
- ✅ Dependency Inversion (зависимости направлены внутрь)
- ✅ Domain не зависит ни от чего

### SOLID Principles
- ✅ Single Responsibility - каждый класс имеет одну ответственность
- ✅ Open/Closed - расширяемость через интерфейсы
- ✅ Liskov Substitution - использование абстракций
- ✅ Interface Segregation - узкие специализированные интерфейсы
- ✅ Dependency Inversion - зависимость от абстракций

### Другие практики
- ✅ Async/await для асинхронности
- ✅ Dependency Injection
- ✅ Options Pattern для конфигурации
- ✅ Structured Logging с Serilog
- ✅ CancellationToken для отмены операций
- ✅ Record types для неизменяемых DTO
- ✅ Nullable Reference Types
- ✅ File-scoped namespaces
- ✅ Modern C# features (C# 13)

## 🔧 Настройка параметров AI

В `appsettings.json` можно настроить поведение AI:

```json
{
  "Ollama": {
    "Model": "deepseek-r1:8b",           // Модель для использования
    "Temperature": 0.7,                   // Креативность (0.0 - 1.0)
    "MaxTokens": 2000,                    // Макс. длина ответа
    "TimeoutSeconds": 120,                // Таймаут запроса
    "SystemPrompt": "Твой промпт..."      // Инструкции для AI
  }
}
```

**Temperature:**
- `0.0` - детерминированные, предсказуемые ответы
- `0.7` - балансная креативность (рекомендуется)
- `1.0` - максимальная креативность

## 📝 Логи

Логи сохраняются в:
- **Консоль** - все сообщения
- **Файлы** - `logs/tg-companion-YYYY-MM-DD.log`

Уровни логирования настраиваются в `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TgCompanion": "Debug"
    }
  }
}
```

## 📚 Документация

В проекте доступна подробная документация:

| Документ | Описание |
|----------|----------|
| [README.md](README.md) | Основная документация проекта |
| [QUICKSTART.md](QUICKSTART.md) | Быстрый старт за 5 минут |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Детальное описание архитектуры |
| [DOCKER.md](DOCKER.md) | Docker инструкции и troubleshooting |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Руководство для контрибьюторов |
| [CHANGELOG.md](CHANGELOG.md) | История версий и изменений |
| [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) | Краткий обзор проекта |

## 🤝 Вклад в проект

Буду рад вашим предложениям и улучшениям! 

1. Fork проекта
2. Создайте feature branch (`git checkout -b feature/amazing-feature`)
3. Commit изменения (`git commit -m 'Add amazing feature'`)
4. Push в branch (`git push origin feature/amazing-feature`)
5. Откройте Pull Request

## 📄 Лицензия

Этот проект создан для демонстрации и обучения. Можете использовать как хотите.

**DeepSeek-R1** распространяется под MIT License.

## 🙏 Благодарности

- [DeepSeek-R1](https://github.com/deepseek-ai/DeepSeek-R1) - отличная reasoning модель
- [Ollama](https://ollama.com/) - удобный инструмент для запуска LLM локально
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - библиотека для Telegram Bot API

## 📞 Поддержка

Если у вас возникли вопросы или проблемы:

1. Проверьте, что Ollama запущен: `ollama list`
2. Проверьте логи в папке `logs/`
3. Убедитесь, что модель загружена: `ollama pull deepseek-r1:8b`
4. Проверьте правильность Bot Token

---

**Создано с ❤️ используя .NET 9 и Clean Architecture**

