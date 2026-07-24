# ADR-002

## Title

Do not use Strongly Typed IDs in Project 1

## Status

Accepted

---

## Context

При проектировании доменной модели возник вопрос:

Использовать ли Strongly Typed IDs
вместо обычного Guid.

Например:

```csharp
public readonly record struct UserId(Guid Value);
```

или

```csharp
public Guid Id { get; private set; }
```

Strongly Typed IDs повышают типобезопасность.

Компилятор не позволит перепутать:

- UserId
- TicketId
- CommentId

Однако они также увеличивают сложность:

- настройки EF Core;
- сериализации;
- маршрутизации;
- Swagger;
- общего количества инфраструктурного кода.

---

## Decision

В первом проекте использовать обычный Guid.

Strongly Typed IDs внедрить
во втором или третьем проекте,
когда базовая архитектура будет освоена.

---

## Consequences

### Плюсы

- проще модель;
- проще EF Core mapping;
- меньше отвлекающих факторов;
- легче сосредоточиться на DDD и Clean Architecture.

### Минусы

Компилятор не сможет защитить
от случайной передачи неправильного идентификатора.

Например:

```csharp
Guid userId;
Guid ticketId;
```

можно случайно перепутать.