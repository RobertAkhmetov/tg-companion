# 🏛️ Архитектура проекта

## Обзор

Telegram AI Companion построен на принципах **Clean Architecture** (Чистая Архитектура) от Robert C. Martin (Uncle Bob).

## 📐 Диаграмма слоев

```
                    ┌─────────────────────────────────────┐
                    │                                     │
                    │         🚀 API LAYER                │
                    │      (Presentation Layer)           │
                    │                                     │
                    │  • BotWorker (Background Service)   │
                    │  • Program.cs (Entry Point)         │
                    │  • DI Configuration                 │
                    │  • Serilog Setup                    │
                    │                                     │
                    └────────────┬────────────────────────┘
                                 │
                                 │ зависит от
                                 ▼
    ┌─────────────────────────────────────────────────────────────┐
    │                                                               │
    │              🔌 INFRASTRUCTURE LAYER                          │
    │           (External Services & Repositories)                 │
    │                                                               │
    │  ┌──────────────────┐  ┌──────────────────┐  ┌────────────┐ │
    │  │ TelegramBotSvc   │  │  OllamaAiService │  │  InMemory  │ │
    │  │  (Telegram API)  │  │   (Ollama API)   │  │ Repository │ │
    │  └──────────────────┘  └──────────────────┘  └────────────┘ │
    │                                                               │
    │  • External API integrations                                 │
    │  • Data persistence                                          │
    │  • Configuration Settings                                    │
    │                                                               │
    └─────────────────────────┬───────────────────────────────────┘
                              │
                              │ зависит от
                              ▼
            ┌────────────────────────────────────────────┐
            │                                            │
            │        💼 APPLICATION LAYER                │
            │        (Business Logic)                    │
            │                                            │
            │  ┌───────────────────────────────────┐    │
            │  │  ProcessMessageUseCase            │    │
            │  │   • Get/Create Conversation       │    │
            │  │   • Add User Message              │    │
            │  │   • Generate AI Response          │    │
            │  │   • Save & Send Response          │    │
            │  └───────────────────────────────────┘    │
            │                                            │
            │  ┌───────────────────────────────────┐    │
            │  │  ClearConversationUseCase         │    │
            │  │   • Delete Conversation           │    │
            │  │   • Send Confirmation             │    │
            │  └───────────────────────────────────┘    │
            │                                            │
            │  • Use Cases (Business Scenarios)         │
            │  • DTOs (Data Transfer Objects)           │
            │  • Orchestration Logic                    │
            │                                            │
            └─────────────────┬──────────────────────────┘
                              │
                              │ зависит от
                              ▼
                ┌───────────────────────────────────┐
                │                                   │
                │      🏛️ DOMAIN LAYER              │
                │     (Core Business Logic)         │
                │                                   │
                │  Entities:                        │
                │  ┌─────────────────────────┐     │
                │  │  ChatMessage            │     │
                │  │  • Id, ChatId, UserId   │     │
                │  │  • Text, Timestamp      │     │
                │  │  • Role (User/Bot)      │     │
                │  └─────────────────────────┘     │
                │                                   │
                │  ┌─────────────────────────┐     │
                │  │  Conversation           │     │
                │  │  • ChatId               │     │
                │  │  • Messages[]           │     │
                │  │  • CreatedAt            │     │
                │  │  • LastActivityAt       │     │
                │  └─────────────────────────┘     │
                │                                   │
                │  Interfaces:                      │
                │  • IAiService                     │
                │  • IConversationRepository        │
                │  • ITelegramBotService            │
                │                                   │
                │  ❌ NO DEPENDENCIES!               │
                │                                   │
                └───────────────────────────────────┘
```

## 🎯 Принцип зависимостей

```
Direction of Dependencies (all pointing INWARD):

API Layer
    ↓ depends on
Infrastructure Layer
    ↓ depends on
Application Layer
    ↓ depends on
Domain Layer (No dependencies!)
```

### Dependency Inversion Principle

Внешние слои зависят от внутренних через **интерфейсы**, определенные в Domain:

```csharp
// Domain определяет интерфейс
namespace TgCompanion.Domain.Interfaces;
public interface IAiService { ... }

// Infrastructure реализует
namespace TgCompanion.Infrastructure.Services;
public class OllamaAiService : IAiService { ... }

// Application использует абстракцию
namespace TgCompanion.Application.UseCases;
public class ProcessMessageUseCase
{
    private readonly IAiService _aiService; // Зависимость от интерфейса!
    
    public ProcessMessageUseCase(IAiService aiService)
    {
        _aiService = aiService;
    }
}
```

## 📦 Слои детально

### 1. 🏛️ Domain Layer (Ядро)

**Ответственность**: Чистая бизнес-логика, правила домена

**Содержит**:
- Entities (сущности)
- Value Objects (опционально)
- Domain Interfaces (контракты)
- Domain Exceptions (опционально)

**Зависимости**: НЕТ! Полностью изолирован.

**Файлы**:
```
TgCompanion.Domain/
├── Entities/
│   ├── ChatMessage.cs      # Сообщение в чате
│   ├── Conversation.cs     # Контекст разговора
│   └── MessageRole.cs      # Enum роли
└── Interfaces/
    ├── IAiService.cs
    ├── IConversationRepository.cs
    └── ITelegramBotService.cs
```

**Пример Entity**:
```csharp
public sealed class Conversation
{
    public long ChatId { get; init; }
    public IReadOnlyList<ChatMessage> Messages { get; }
    
    public void AddMessage(ChatMessage message)
    {
        // Бизнес-правило: сообщение должно быть из этого чата
        if (message.ChatId != ChatId)
            throw new InvalidOperationException("...");
            
        _messages.Add(message);
    }
}
```

### 2. 💼 Application Layer (Бизнес-логика)

**Ответственность**: Координация бизнес-сценариев (Use Cases)

**Содержит**:
- Use Cases
- DTOs
- Application Services
- Mapping Logic

**Зависимости**: Domain

**Файлы**:
```
TgCompanion.Application/
├── DTOs/
│   └── TelegramMessageDto.cs
├── UseCases/
│   ├── ProcessMessageUseCase.cs
│   └── ClearConversationUseCase.cs
└── DependencyInjection.cs
```

**Пример Use Case**:
```csharp
public sealed class ProcessMessageUseCase
{
    private readonly IAiService _aiService;
    private readonly IConversationRepository _repository;
    
    public async Task ExecuteAsync(TelegramMessageDto dto)
    {
        // 1. Get/Create conversation
        var conversation = await _repository.GetByChatIdAsync(dto.ChatId)
                           ?? Conversation.Create(dto.ChatId);
        
        // 2. Add user message
        var userMessage = ChatMessage.CreateUserMessage(...);
        conversation.AddMessage(userMessage);
        
        // 3. Generate AI response
        var response = await _aiService.GenerateResponseAsync(...);
        
        // 4. Add bot message
        var botMessage = ChatMessage.CreateAssistantMessage(...);
        conversation.AddMessage(botMessage);
        
        // 5. Save
        await _repository.SaveAsync(conversation);
    }
}
```

### 3. 🔌 Infrastructure Layer (Интеграции)

**Ответственность**: Реализация внешних интеграций

**Содержит**:
- External Service Clients (Telegram, Ollama)
- Data Access (Repositories)
- Configuration Classes
- External Libraries Integration

**Зависимости**: Application, Domain

**Файлы**:
```
TgCompanion.Infrastructure/
├── Configuration/
│   ├── OllamaSettings.cs
│   └── TelegramSettings.cs
├── Services/
│   ├── OllamaAiService.cs       # Implements IAiService
│   └── TelegramBotService.cs    # Implements ITelegramBotService
├── Repositories/
│   └── InMemoryConversationRepository.cs  # Implements IConversationRepository
└── DependencyInjection.cs
```

**Пример Service**:
```csharp
public sealed class OllamaAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    
    public async Task<string> GenerateResponseAsync(...)
    {
        // Вызов внешнего Ollama API
        var response = await _httpClient.PostAsJsonAsync(...);
        return await response.Content.ReadFromJsonAsync<...>();
    }
}
```

### 4. 🚀 API Layer (Presentation)

**Ответственность**: Точка входа, конфигурация приложения

**Содержит**:
- Entry Point (Program.cs)
- Background Services / Workers
- Dependency Injection Configuration
- Middleware (если Web API)
- Configuration Files

**Зависимости**: Infrastructure

**Файлы**:
```
TgCompanion.Api/
├── Program.cs                  # Entry point, DI setup
├── BotWorker.cs                # Background service
├── appsettings.json            # Configuration
└── appsettings.Development.json
```

**Пример Worker**:
```csharp
public sealed class BotWorker : BackgroundService
{
    private readonly ITelegramBotService _botService;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Проверка зависимостей
        await CheckDependencies();
        
        // Запуск бота
        await _botService.StartReceivingAsync(ct);
        
        // Ожидание остановки
        await Task.Delay(Timeout.Infinite, ct);
    }
}
```

## 🔄 Поток данных

### Обработка сообщения (полный цикл)

```
1. Telegram User отправляет сообщение
        ↓
2. Telegram Bot API → Webhook/Long Polling
        ↓
3. TelegramBotService (Infrastructure)
   - Получает Update
   - Парсит сообщение
        ↓
4. ProcessMessageUseCase (Application)
   - Координирует бизнес-логику
        ↓
5. IConversationRepository (Domain Interface → Infrastructure Implementation)
   - Получает/создает Conversation
        ↓
6. Conversation.AddMessage() (Domain Entity)
   - Валидирует и добавляет сообщение
        ↓
7. IAiService (Domain Interface → Infrastructure Implementation)
   - Генерирует ответ через Ollama
        ↓
8. Conversation.AddMessage() (Domain Entity)
   - Добавляет ответ бота
        ↓
9. IConversationRepository (Domain Interface → Infrastructure Implementation)
   - Сохраняет Conversation
        ↓
10. ITelegramBotService (Domain Interface → Infrastructure Implementation)
    - Отправляет ответ пользователю
        ↓
11. Telegram User получает ответ
```

## 🎨 Паттерны проектирования

### 1. **Repository Pattern**

```csharp
// Интерфейс в Domain
public interface IConversationRepository
{
    Task<Conversation?> GetByChatIdAsync(long chatId);
    Task SaveAsync(Conversation conversation);
}

// Реализация в Infrastructure
public class InMemoryConversationRepository : IConversationRepository
{
    private readonly ConcurrentDictionary<long, Conversation> _storage = new();
    
    // Implementation...
}
```

**Преимущества**:
- Абстракция хранилища
- Легко заменить на другую реализацию (SQL, Redis, etc.)
- Тестируемость

### 2. **Use Case Pattern**

```csharp
// Каждый бизнес-сценарий = отдельный класс
public sealed class ProcessMessageUseCase
{
    public async Task ExecuteAsync(TelegramMessageDto dto)
    {
        // Полная бизнес-логика одного сценария
    }
}
```

**Преимущества**:
- Single Responsibility
- Понятная бизнес-логика
- Легко тестировать

### 3. **Options Pattern**

```csharp
// Configuration класс
public sealed class OllamaSettings
{
    public string BaseUrl { get; set; }
    public string Model { get; set; }
}

// Регистрация
services.Configure<OllamaSettings>(
    configuration.GetSection("Ollama"));

// Использование
public class OllamaAiService
{
    public OllamaAiService(IOptions<OllamaSettings> options)
    {
        _settings = options.Value;
    }
}
```

**Преимущества**:
- Типобезопасная конфигурация
- Валидация на старте
- Hot reload (опционально)

### 4. **Dependency Injection**

```csharp
// Регистрация
services.AddScoped<ProcessMessageUseCase>();
services.AddSingleton<ITelegramBotService, TelegramBotService>();

// Использование через конструктор
public class BotWorker
{
    public BotWorker(ITelegramBotService botService)
    {
        _botService = botService;
    }
}
```

**Преимущества**:
- Loose coupling
- Тестируемость
- Управление жизненным циклом

## ✅ SOLID Principles

### Single Responsibility

Каждый класс имеет одну ответственность:
- `ProcessMessageUseCase` - только обработка сообщений
- `OllamaAiService` - только интеграция с Ollama
- `Conversation` - только управление историей

### Open/Closed

Легко расширяется через интерфейсы:
```csharp
// Хотите другой AI? Просто реализуйте IAiService!
public class OpenAiService : IAiService { }
public class AnthropicService : IAiService { }
```

### Liskov Substitution

Любая реализация интерфейса взаимозаменяема:
```csharp
IAiService service = new OllamaAiService();
// или
IAiService service = new OpenAiService();
// Код работает одинаково!
```

### Interface Segregation

Узкие специализированные интерфейсы:
```csharp
public interface IAiService { } // Только AI операции
public interface IConversationRepository { } // Только хранение
public interface ITelegramBotService { } // Только Telegram
```

### Dependency Inversion

Зависимость от абстракций, не от конкретных реализаций:
```csharp
// ✅ Правильно
public ProcessMessageUseCase(IAiService aiService) { }

// ❌ Неправильно
public ProcessMessageUseCase(OllamaAiService aiService) { }
```

## 🧪 Тестируемость

Благодаря Clean Architecture, легко тестировать:

```csharp
// Unit тест Use Case
[Fact]
public async Task ProcessMessage_ShouldGenerateResponse()
{
    // Arrange
    var mockAiService = new Mock<IAiService>();
    var mockRepository = new Mock<IConversationRepository>();
    var useCase = new ProcessMessageUseCase(mockAiService.Object, ...);
    
    // Act
    await useCase.ExecuteAsync(new TelegramMessageDto(...));
    
    // Assert
    mockAiService.Verify(x => x.GenerateResponseAsync(...));
}
```

## 🔧 Расширение архитектуры

### Добавление нового слоя (Tests)

```
├── src/                    # Production code
│   └── ...
└── tests/                  # Test projects
    ├── TgCompanion.Domain.Tests/
    ├── TgCompanion.Application.Tests/
    └── TgCompanion.Integration.Tests/
```

### Вертикальное разделение (Feature Slices)

Для больших проектов можно организовать по фичам:
```
Application/
├── Messages/
│   ├── ProcessMessage/
│   └── ClearConversation/
└── Statistics/
    └── GetStatistics/
```

## 📚 Дополнительные материалы

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Архитектура проекта следует лучшим практикам индустрии! 🏛️✨**

