# 🤝 Вклад в проект

Спасибо за интерес к проекту! Мы рады любому вкладу - от исправления опечаток до новых функций.

## Как внести вклад

### 1. Сообщить о проблеме (Issue)

Нашли баг или есть идея? Создайте Issue:

1. Проверьте, что похожая проблема еще не создана
2. Используйте понятный заголовок
3. Опишите проблему детально:
   - Шаги для воспроизведения
   - Ожидаемое поведение
   - Реальное поведение
   - Версия .NET и ОС
   - Логи (если есть)

### 2. Предложить изменения (Pull Request)

#### Процесс

1. **Fork** репозитория
2. **Clone** вашего fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/tg-companion.git
   cd tg-companion
   ```

3. Создайте **feature branch**:
   ```bash
   git checkout -b feature/my-awesome-feature
   ```

4. Внесите изменения и **commit**:
   ```bash
   git add .
   git commit -m "Add: описание изменений"
   ```

5. **Push** в ваш fork:
   ```bash
   git push origin feature/my-awesome-feature
   ```

6. Откройте **Pull Request** на GitHub

#### Требования к коду

- ✅ Код должен компилироваться без ошибок
- ✅ Следуйте существующему стилю кода
- ✅ Используйте осмысленные имена переменных и методов
- ✅ Добавьте XML-комментарии к публичным методам
- ✅ Соблюдайте принципы SOLID и Clean Architecture

#### Commit сообщения

Используйте понятный формат:

```
Add: новая функциональность
Fix: исправление бага
Update: обновление существующего кода
Refactor: рефакторинг без изменения функциональности
Docs: обновление документации
Style: форматирование, отступы и т.д.
```

Примеры:
```
Add: support for streaming responses from Ollama
Fix: null reference exception in conversation repository
Update: Telegram.Bot library to version 22.0
Docs: improve README installation instructions
```

## Что можно улучшить

### Идеи для вклада

#### 🐛 Исправления
- Обработка edge cases
- Улучшение error handling
- Исправление багов

#### ✨ Новые функции
- Персистентное хранилище (Database) вместо in-memory
- Поддержка групповых чатов
- Команда для изменения настроек AI (temperature, model)
- Поддержка вложений (картинки, файлы)
- Rate limiting для предотвращения спама
- Статистика использования бота
- Multi-language support

#### 📖 Документация
- Улучшение README
- Примеры использования
- Видео-туториалы
- Перевод на другие языки

#### 🏗️ Архитектура
- Unit тесты
- Integration тесты
- Benchmark тесты
- CI/CD pipeline
- Kubernetes deployment

#### 🎨 Улучшения кода
- Performance оптимизации
- Рефакторинг
- Обновление зависимостей
- Улучшение логирования

## Стиль кода

### C# Guidelines

Проект следует стандартным .NET coding conventions:

```csharp
// ✅ Правильно
public sealed class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public async Task DoSomethingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Doing something...");
        // implementation
    }
}

// ❌ Неправильно
public class myService
{
    private ILogger logger;
    
    public void DoSomething()
    {
        // no async, no logging, bad naming
    }
}
```

### Ключевые принципы

1. **Async/Await** - все IO операции асинхронные
2. **CancellationToken** - поддержка отмены
3. **Logging** - логируйте важные операции
4. **Null safety** - используйте `nullable reference types`
5. **Dependency Injection** - все зависимости через конструктор
6. **SOLID** - следуйте принципам
7. **Clean Architecture** - соблюдайте границы слоев

## Структура проекта

```
tg-companion/
├── src/
│   ├── TgCompanion.Domain/          # Бизнес-логика, entities
│   ├── TgCompanion.Application/     # Use cases, DTOs
│   ├── TgCompanion.Infrastructure/  # Внешние интеграции
│   └── TgCompanion.Api/            # Точка входа, DI
├── tests/                           # (будущие тесты)
├── docs/                            # (будущая документация)
└── README.md
```

### Добавление новой функции

Пример: Добавить команду `/stats` для статистики

1. **Domain** - создать entity `BotStatistics`
2. **Domain** - добавить интерфейс `IStatisticsRepository`
3. **Application** - создать `GetStatisticsUseCase`
4. **Infrastructure** - реализовать `InMemoryStatisticsRepository`
5. **Infrastructure** - обработать команду в `TelegramBotService`
6. **Api** - зарегистрировать зависимости в DI

## Тестирование

Перед отправкой PR:

```bash
# Убедитесь, что проект компилируется
dotnet build

# Запустите проект
cd src/TgCompanion.Api
dotnet run

# Протестируйте основные сценарии
# - Отправка сообщения боту
# - Команды /start, /help, /clear
# - Проверка логов
```

## Вопросы?

- Создайте Issue с тегом `question`
- Опишите, что вы пытаетесь сделать
- Мы постараемся помочь!

## Кодекс поведения

- Будьте вежливы и уважительны
- Конструктивная критика приветствуется
- Помогайте другим участникам
- Фокусируйтесь на коде, а не на людях

---

**Спасибо за вклад в проект! 🙏✨**

