# 🚀 Быстрый старт

Эта инструкция поможет вам запустить бота за 5 минут!

## Шаг 1: Предварительные требования

### Проверьте, что установлено:

```bash
# .NET 9 SDK
dotnet --version
# Должно быть: 9.0.x

# Ollama
ollama --version
# Если не установлен, скачайте с https://ollama.com/download
```

## Шаг 2: Скачайте модель DeepSeek-R1

```bash
# Рекомендуемая модель (5.2 GB)
ollama pull deepseek-r1:8b

# Проверьте, что модель загружена
ollama list
```

**💡 Совет:** Если у вас меньше 16 GB RAM, используйте `deepseek-r1:1.5b` (1.1 GB)

## Шаг 3: Создайте Telegram бота

1. Откройте Telegram и найдите [@BotFather](https://t.me/botfather)
2. Отправьте команду: `/newbot`
3. Введите имя бота (например: "My AI Companion")
4. Введите username бота (должен заканчиваться на "bot", например: "my_ai_companion_bot")
5. **Скопируйте Bot Token** - он выглядит так: `1234567890:ABCdefGHIjklMNOpqrsTUVwxyz`

## Шаг 4: Настройте проект

### Вариант А: Через appsettings.json (Простой)

1. Откройте `src/TgCompanion.Api/appsettings.json`
2. Замените `"BotToken"` на ваш токен:

```json
{
  "Telegram": {
    "BotToken": "1234567890:ABCdefGHIjklMNOpqrsTUVwxyz",
    "BotUsername": "my_ai_companion_bot"
  }
}
```

### Вариант Б: Через User Secrets (Безопасный)

```bash
cd src/TgCompanion.Api

dotnet user-secrets set "Telegram:BotToken" "1234567890:ABCdefGHIjklMNOpqrsTUVwxyz"
dotnet user-secrets set "Telegram:BotUsername" "my_ai_companion_bot"
```

## Шаг 5: Запустите бота

```bash
# Из корневой папки проекта
cd src/TgCompanion.Api
dotnet run
```

Вы должны увидеть:

```
[INF] Starting Telegram Companion Bot
[INF] Ollama service is available
[INF] Starting bot @my_ai_companion_bot
[INF] Bot started successfully
```

## Шаг 6: Протестируйте бота

1. Откройте Telegram
2. Найдите вашего бота по username (например: `@my_ai_companion_bot`)
3. Нажмите **Start** или отправьте `/start`
4. Напишите любое сообщение, например: "Привет!"

Бот должен ответить! 🎉

## Полезные команды бота

```
/start  - Начать работу с ботом
/help   - Показать справку
/clear  - Очистить историю разговора
/about  - Информация о боте
```

## 🐛 Решение проблем

### Ошибка: "Ollama service is not available"

**Причина:** Ollama не запущен или недоступен

**Решение:**
```bash
# Проверьте статус Ollama
ollama list

# Попробуйте загрузить модель снова
ollama pull deepseek-r1:8b

# Проверьте, что Ollama доступен
curl http://localhost:11434/api/tags
```

### Ошибка: "Telegram bot token is not configured"

**Причина:** Не указан токен бота

**Решение:** Повторите Шаг 4

### Бот не отвечает на сообщения

**Проверьте:**

1. Бот запущен? (должны быть логи в консоли)
2. Ollama запущен? (`ollama list`)
3. Токен правильный? (проверьте в appsettings.json)
4. Нет ошибок в логах? (смотрите консоль и папку `logs/`)

### Бот отвечает слишком медленно

**Решение:** Используйте более легкую модель

```bash
ollama pull deepseek-r1:1.5b
```

Затем в `appsettings.json`:
```json
{
  "Ollama": {
    "Model": "deepseek-r1:1.5b"
  }
}
```

## 🎯 Следующие шаги

- Прочитайте полный [README.md](README.md) для подробной информации
- Настройте параметры AI в `appsettings.json` (temperature, max tokens)
- Измените системный промпт для кастомизации поведения бота
- Посмотрите логи в папке `logs/` для отладки

## 📞 Нужна помощь?

Откройте Issue в GitHub или проверьте логи:

```bash
# Логи в консоли
# Файлы логов в папке logs/tg-companion-YYYY-MM-DD.log
```

---

**Готово! Наслаждайтесь общением с вашим AI-ботом! 🤖✨**

