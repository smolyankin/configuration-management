# API Управления Конфигурациями

REST API для управления пользовательскими конфигурациями с уведомлениями в реальном времени через SignalR.

## Функциональность

- **Управление конфигурациями**: Создание, чтение и обновление конфигураций
- **Версионирование конфигураций**: Автоматическое версионирование изменений с возможностью отката
- **Уведомления в реальном времени**: Подписки на обновления конфигураций через SignalR

## Технологический стек

- **.NET 10** с ASP.NET Core
- **PostgreSQL 18** с поддержкой JSONB
- **SignalR** для real-time коммуникаций
- **MediatR** для реализации паттерна CQRS
- **Serilog** для структурированного логирования
- **FluentValidation** для валидации
- **Redis** для кэширования и масштабирования SignalR
- **Docker** и Docker Compose для контейнеризации
- **xUnit**, **Moq**, **FluentAssertions** для тестирования

## Предварительные требования

- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL 18 (или Docker)
- Redis (или Docker)

## Быстрый старт

### Использование Docker (Рекомендуется)

1. Клонирование репозитория:
   ```bash
   git clone <repository-url>
   cd configuration-management
   ```
   
2. Запуск всех сервисов:
   ```bash
   docker-compose -f docker/docker-compose.yml up -d
   ```

3. API будет доступен по адресу `http://localhost:5000`
   - Swagger документация: `http://localhost:5000/swagger`
   - Проверка здоровья: `http://localhost:5000/health`

## API документация

### Основные эндпоинты

- **Проверка здоровья**: `GET /health`
- **Информация**: `GET /`
- **Регистрация**: `POST /api/Auth/register`
- **Вход**: `POST /api/Auth/login`

### Эндпоинты конфигураций

- **Создать конфигурацию**: `POST /api/Configurations`
- **Получить конфигурацию**: `GET /api/Configurations/{id}`
- **Получить все конфигурации**: `GET /api/Configurations`
- **Обновить конфигурацию**: `PUT /api/Configurations/{id}`
- **Откатить к версии**: `POST /api/Configurations/{id}/restore`

### Эндпоинты уведомлений

- **Подписаться**: `POST /api/NotificationSubscriptions/subscribe`
- **Отписаться**: `POST /api/NotificationSubscriptions/unsubscribe`
- 
### SignalR Hub

Подключитесь к ConfigurationHub для real-time обновлений:
- **URL хаба**: `/hubs/notifications`

## Тестирование

### Использование Postman

Для тестирования API используйте готовую коллекцию Postman:

1. Импортируйте коллекцию из файла `ConfigurationManagement.postman_collection.json`
2. Настройте переменные окружения в Postman:
   - `token`: JWT токен (получается после логина)
   - `configurationId`: ID конфигурации для тестов (получается после создания конфигурации)
3. Создайте подключение WebSocket
   - ws://localhost:5000/hubs/notifications
   - Headers: Authorization: Bearer [token]
   - Message: {"protocol":"json","version":1} + символ https://unicodeplus.com/U+001E
   - Отправьте сообщение для подключения (после получения JWT токена)

4. Последовательность тестирования:
   - Создайте пользователя через `Auth/register`
   - Получите JWT токен через `Auth/login`
   - Установите токен в переменную `token`
   - Подпишитесь на уведомления `NotificationSubscriptions/subscribe`
   - Создайте конфигурацию через `Configurations`
   - Установите ид конфигурации в переменную `configurationId`
   - Тестируйте

## Возможные улучшения сервиса

### Логирование

1. Добавление correlationId
2. Асинхронная реализация

### SignalR

1. Использование Redis
1. Использование Concurrency
2. Ограничение количества соединений на пользователя
3. Внедрение Distributed tracing

### Прочее

1. Единый стиль ответов методов API с детализацией и traceId
2. Расширенные HealthChecks
3. Кеширование с использованием Redis
4. Внедрение подхода SoftDelete
5. Внедрение Outbox Pattern с retry для асинхронной отправки уведомлений в SignalR

## Структура проекта

```
src/
├── ConfigurationManagement.Api/          # Web API слой
├── ConfigurationManagement.Application/  # Application слой
├── ConfigurationManagement.Domain/       # Domain слой
├── ConfigurationManagement.Infrastructure/ # Infrastructure слой
└── ConfigurationManagement.Tests/        # Тестовые проекты
    ├── ConfigurationManagement.Tests.Unit/
    └── ConfigurationManagement.Tests.Integration/
```
