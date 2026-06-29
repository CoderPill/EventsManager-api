# EventsManager API

API backend para gestión de eventos, venues y reservas con reglas de negocio de capacidad, horarios, confirmación y cancelación. Implementa Clean Architecture pragmática con Vertical Slices, patrón `Result<T>` para control de flujo sin excepciones, autenticación JWT con revocación en memoria y validación FluentValidation por feature.

---

## 1. Ejecución local

### 1.1 Prerrequisitos
- .NET 10 SDK
- SQL Server (LocalDB, Express o Docker)
- Terminal PowerShell (Windows) / bash (Linux/macOS)

### 1.2 Variables de entorno (secretos)
La aplicación carga configuración desde variables de entorno con prefijo `EVENTSMANAGER_` (definido en `SystemValues.Infrastructure.EnvironmentPrefix`). El doble guion bajo `__` representa jerarquía anidada en el binding de configuración.

Ejecutar en PowerShell como administrador:
```powershell
setx EVENTSMANAGER_ConnectionStringsSettings__DefaultConnection "Server=localhost;Database=EventsManagerDb;User Id=usuario;Password=contraseña;TrustServerCertificate=True;" /m
setx EVENTSMANAGER_AdminSeedSettings__Username "Sysadmin" /m
setx EVENTSMANAGER_AdminSeedSettings__Password "123" /m
setx EVENTSMANAGER_JwtSettings__SecretKey "aB3zxY9!mK2#pL5@vR8nW1cZ4gA7dH0jF6sT9uI2oE5wQ8" /m
setx EVENTSMANAGER_SmtpSettings__SmtpUsername "usuario smtp" /m
setx EVENTSMANAGER_SmtpSettings__SmtpPassword "contraseña smtp" /m
setx EVENTSMANAGER_SmtpSettings__FromEmail "El correo de origen" /m
setx EVENTSMANAGER_SmtpSettings__FromName "EventsManager" /m
```
> Reiniciar terminal/IDE tras ejecutar para que las variables queden disponibles.

| Variable | Clase Settings | Descripción |
|----------|----------------|-------------|
| `EVENTSMANAGER_ConnectionStringsSettings__DefaultConnection` | `ConnectionStringsSettings` | Cadena de conexión SQL Server |
| `EVENTSMANAGER_AdminSeedSettings__Username` | `AdminSeedSettings` | Usuario administrador inicial |
| `EVENTSMANAGER_AdminSeedSettings__Password` | `AdminSeedSettings` | Contraseña administrador inicial |
| `EVENTSMANAGER_JwtSettings__SecretKey` | `JwtSettings` | Clave secreta HMAC-SHA256 (≥256 bits) |
| `EVENTSMANAGER_SmtpSettings__SmtpUsername` | `SmtpSettings` | Usuario SMTP |
| `EVENTSMANAGER_SmtpSettings__SmtpPassword` | `SmtpSettings` | Contraseña SMTP (si usa Gmail/Outlook personal: **App Password**, no contraseña de cuenta) |
| `EVENTSMANAGER_SmtpSettings__FromEmail` | `SmtpSettings` | Email remitente |
| `EVENTSMANAGER_SmtpSettings__FromName` | `SmtpSettings` | Nombre remitente visible (opcional, se usa FromName del JSON si no se define) |

### 1.3 Configuración no sensible (appsettings.json)
Settings no secretos se definen en `appsettings.json` / `appsettings.Development.json`. La precedencia es: **Environment Variables > appsettings.json > defaults**.

```json
{
  "JwtSettings": {
    "ExpirationInHours": 24
  },
  "SmtpSettings": {
    "Enabled": false,
    "Host": "smtp.example.com",
    "Port": 587,
    "EnableSsl": true,
    "FromName": "EventsManager"
  },
  "AllowedOriginCORS": [
    "http://localhost:4200"
  ]
}
```

| Clase | Propiedades | Origen típico |
|-------|-------------|---------------|
| `JwtSettings` | `SecretKey`, `ExpirationInHours` | Env var (SecretKey) + JSON (ExpirationInHours) |
| `SmtpSettings` | `Enabled`, `Host`, `Port`, `EnableSsl`, `SmtpUsername`, `SmtpPassword`, `FromEmail`, `FromName` | Env vars (credenciales) + JSON (Host/Port/Enabled/EnableSsl/FromName) |
| `AdminSeedSettings` | `Username`, `Password` | Env vars obligatorias |
| `ConnectionStringsSettings` | `DefaultConnection` | Env var obligatoria |

### 1.4 Ejecutar y verificar
```bash
cd src/EventsManager.Api
dotnet run
```
Al arrancar la aplicación (`Program.cs` → `ApplyMigrationsAndSeedAsync`):
1. Aplica migraciones EF Core pendientes
2. Ejecuta seed inicial: usuario admin (`Sysadmin`/`123`) + venues por defecto
3. Inicia host en `https://localhost:{puerto}`

Verificación:
- Swagger UI: `https://localhost:{puerto}/swagger`
- Health check: `GET /` → `"API Running."`

---

## 2. Arquitectura y justificación

### 2.1 Capas y responsabilidades

| Capa | Proyecto | Responsabilidad | Dependencias |
|------|----------|-----------------|--------------|
| **Core** | `EventsManager.Core` | Entidades, enums, constantes, `IDateTimeProvider` | Ninguna (dominio puro) |
| **Application** | `EventsManager.Application` | Casos de uso, puertos (interfaces), DTOs, validación, `Result<T>`, `BaseUseCase` | Core |
| **Infrastructure** | `EventsManager.Infrastructure` | EF Core, repositorios, JWT (MemoryCache), Email (SMTP/Null), Hashing (BCrypt), CodeGen, Logging, Seeders | Application, Core |
| **API** | `EventsManager.Api` | Controllers, Middleware, DI, Swagger, Serialización | Application, Infrastructure |

### 2.2 Flujo de request
```
HTTP Request
    │
    ▼
Controller (API) ──▶ BaseApiController (Authorize global, [AllowAnonymous] selectivo)
    │
    ▼
Handler (Application) ──▶ BaseUseCase<TRequest,TResponse>
    │                        │
    │                        ├── ValidateAsync (FluentValidation)
    │                        └── OnExecute (lógica de negocio)
    │
    ├── Repositorios (puertos: IEventRepository, IReservationRepository, etc.)
    ├── Servicios (puertos: IJwtService, IEmailService, IAlphaNumericCodeGenerator, IDateTimeProvider)
    │
    ▼
Infrastructure (implementaciones concretas)
    │
    ▼
Result<T> ──▶ Controller ──▶ ResultActionExtensions.ToActionResult() ──▶ HTTP Response
```

### 2.3 Decisiones y tradeoffs

| Decisión | Qué se ganó | Qué se perdió / Costo | Alternativas consideradas |
|----------|-------------|----------------------|---------------------------|
| **Clean Architecture + Vertical Slices** | Aislamiento de dominio, testabilidad sin infraestructura, localización de cambios por feature | Más archivos/boilerplate inicial vs capa de servicios monolítica | Layered tradicional (Services/Repositories), Modular Monolith |
| **Result<T> (struct inmutable) vs Excepciones** | Control de flujo explícito, sin stack trace overhead, fuerza manejo de error en callers, serializable directo a JSON | Verbosidad en callers (`.BindAsync`, pattern matching), no captura contextos de fallo inesperado | Excepciones + Global filter, `OneOf<T>`, `ErrorOr` |
| **Puertos/Adaptadores (Interfaces en Application)** | Inversión de dependencias real, tests con mocks, swap de proveedores (SMTP→NullEmailService) | Indirección extra, más interfaces que mantener | Servicios concretos inyectados directamente, MediatR pipeline behaviors |
| **JWT Revocación en MemoryCache** | Simplicidad operativa, cero dependencias externas, expiración automática por TTL | No persiste reinicios, no escala multi-instancia sin Redis distribuido | Redis (distributed cache), tabla DB `RevokedTokens`, short-lived access + refresh tokens |
| **Vertical Slices (Handler+Request+Validator+DTOs por feature)** | Cohesión alta, acoplamiento bajo, navegación trivial, sin "god services" | Duplicación de patrones (validator, mapper) por feature, menos reuso transversal | MediatR + CQRS clásico, Application Services monolíticos |
| **Nota sobre CQRS** | El sistema **no implementa CQRS explícito** (no hay `ICommand`/`IQuery` separados, ni `CommandHandler`/`QueryHandler` distintos). `BaseUseCase<TRequest,TResponse>` sirve tanto para comandos (escritura) como consultas (lectura). La separación es por feature (Vertical Slices), no por tipo de operación. CQRS clásico fue evaluado como alternativa. | — | — |
| **FluentValidation vs DataAnnotations** | Reglas complejas expresivas, testables aisladamente, separadas del modelo | Dependencia extra, aprendizaje inicial | DataAnnotations + IValidatableObject, manual validation |
| **TimeZone hardcoded (Colombia) en SystemDateTimeProvider** | Correcto para dominio actual, sin config extra | No portable a otros países/zonas sin cambio de código | `IDateTimeProvider` configurable por tenant/setting, UTC everywhere + conversión en presentation |
| **BaseRepository<T> genérico + BuildQuery** | DRY en CRUD, includes/predicate/no-tracking parametrizables | Fuga de abstracción EF Core (`IQueryable`, `Expression`), acopla a EF Core | Repository estricto (solo métodos de dominio), Specification pattern, Dapper |
| **Seed automático al arranque (`ApplyMigrationsAndSeedAsync`)** | Entorno consistente en dev/CI, cero pasos manuales | Arranque más lento, riesgo en prod si migraciones fallan | Migraciones CLI separadas, seed opcional via flag, herramientas externas (DbUp, Flyway) |
| **GlobalExceptionMiddleware** | Manejo centralizado de errores, logging consistente, respuesta uniforme `Result<T>`, evita try/catch dispersos | Oculta stack trace real en producción (solo mensaje genérico), acopla formato de error a `Result<T>` | Filtros de excepción por controller, `UseExceptionHandler` genérico, ProblemDetails (RFC 7807) |
| **Separación secretos en variables de entorno** | Credenciales fuera del repo, cumplimiento 12-factor, distinción clara secretos vs config | Requiere setup manual en cada máquina/CI, dependencia de shell/OS, riesgo de typos en nombres(la estructura de la variable esta ligada a la estructura de la clase settings correspondiente) | `appsettings.json` con user-secrets (dev), Azure Key Vault / AWS Secrets Manager (prod), Docker secrets |
| **Logging de errores en archivos .txt** | Simplicidad operativa, cero dependencias externas, rotación diaria automática, thread-safe con `SemaphoreSlim` | Hardcoded: ruta `ExceptionLogs/`, nombre `Errors-yyyy-MM-dd.txt`, formato plano; no estructurado (no JSON), no índices, no búsqueda, no escalable multi-instancia | Serilog/NLog + sinks (file, seq, elasticsearch), structured logging (JSON), OpenTelemetry, Loki/Grafana |

---

## 3. Tecnologías utilizadas

| Categoría | Tecnología | Versión / Uso |
|-----------|------------|---------------|
| **Runtime** | .NET | 10.0 |
| **Framework** | ASP.NET Core | Controllers , DI built-in |
| **ORM** | Entity Framework Core | SQL Server, Code-First, Migraciones automáticas |
| **Validación** | FluentValidation | Un `AbstractValidator<TRequest>` por feature |
| **Auth** | JWT Bearer | HMAC-SHA256, claims: `jti`, `nameid`, `name`, `role` |
| **Revocación** | `IMemoryCache` | In-memory, expiración absoluta por token |
| **Hashing** | BCrypt | `IPasswordHasher` via `PasswordHasher` |
| **Email** | SMTP / NullObject | `IEmailService` + `NullEmailService` si `Enabled=false`. **Nota**: con Gmail/Outlook personal usar **App Password** (no contraseña de cuenta), habilitar 2FA y generar en seguridad de la cuenta. |
| **Generación códigos** | Alfanumérico custom | `IAlphaNumericCodeGenerator` → `AlphaNumericCodeGenerator` (formato `EV-XXXXXX`) |
| **Serialización** | `System.Text.Json` | camelCase, enums as string (`JsonStringEnumConverter`), case-insensitive |
| **Docs** | Swashbuckle (Swagger) | Bearer auth scheme, XML comments (`GenerateDocumentationFile=true`) |
| **Logging errores** | File-based | `ExceptionLogs/Errors-yyyy-MM-dd.txt`, `SemaphoreSlim` thread-safe |
| **Zona horaria** | `TimeZoneInfo` | Colombia (`SA Pacific Standard Time` Windows / `America/Bogota` Linux)/Docker|

---

## 4. Modelo de dominio

### 4.1 Entidades

| Entidad | Campos clave | Reglas de integridad |
|---------|--------------|---------------------|
| `UserEntity` | `Username`, `PasswordHash`, `Role` (Admin/Organizer/Client) | Username único |
| `VenueEntity` | `Name` (≤32), `Capacity` (>0), `City` (≤32) | - |
| `EventEntity` | `Title` (5-100), `Description` (10-500), `VenueId`, `MaxCapacity` (>0), `StartDate`/`EndDate`, `Price` (>0), `EventType`, `Status`, `IsActive` | Capacidad ≤ Venue; no solape en venue; fin de semana ≤22:00; EndDate > StartDate |
| `ReservationEntity` | `EventId`, `Quantity` (≥1), `BuyerName` (≤32), `BuyerEmail` (email, ≤64), `Status`, `ReservationCode` (`EV-XXXXXX`), `CancelDate`, `HasPenalty` | Solo eventos activos/futuros; ≥1h antes; capacidad disponible |

### 4.2 Enums
| Enum | Valores |
|------|---------|
| `EventType` | `Conferencia=1`, `Taller=2`, `Concierto=3` |
| `EventStatus` | `Activo=1`, `Cancelado=2`, `Completado=3` |
| `ReservationStatus` | `PendientePago=1`, `Confirmada=2`, `Cancelada=3` |
| `UserRole` | `Admin`|

### 4.3 Constantes de negocio (`SystemValues`)
| Constante | Valor | Uso |
|-----------|-------|-----|
| `SecondsPerHour` | 3600 | Cálculos de tiempo |
| `HoursBeforeStartForLastDayRules` | 24 | Límite 5 entradas si <24h |
| `MaxQuantityForLastDay` | 5 | Regla de última hora |
| `ExpensiveEventPriceThreshold` | 100m | Límite 10 entradas si precio >$100 |
| `MaxQuantityForExpensiveEvent` | 10 | Regla evento costoso |
| `HoursBeforeStartForCancellationPenalty` | 48 | Penalización cancelación <48h |
| `EnvironmentPrefix` | `EVENTSMANAGER_` | Prefijo variables de entorno |
| `ReservationCodePrefix` | `EV-` | Prefijo códigos de reserva |
| `CORSPolicyName` | `AllowedOriginCORS` | Nombre política CORS |
| `JsonContentType` | `application/json` | Content-Type respuestas JSON |

---

## 5. Organización por Vertical Slices

Cada feature agrupa su lógica en **Application** e **Infrastructure**:

**Application** (`Features/{FeatureName}/{Operation}/`):
```
Features/{FeatureName}/{Operation}/
├── {Operation}Handler.cs      # Hereda BaseUseCase<TRequest,TResponse>
├── {Operation}Request.cs      # Record con datos de entrada
├── {Operation}Validator.cs    # AbstractValidator<TRequest>
├── {Operation}RequestValidator.cs # (alias en algunas features)
└── DTOs / Mappers / UseCases  # Según necesidad
```

**Infrastructure** (`Persistence/Features/{FeatureName}/` + `Tools/`):
```
Persistence/Features/{FeatureName}/
├── {FeatureName}Repository.cs      # Implementa I{FeatureName}Repository
├── {FeatureName}Configuration.cs   # EF Core IEntityTypeConfiguration
└── {FeatureName}Seeder.cs          # Datos iniciales (si aplica)

Tools/
├── Email/EmailService.cs           # IEmailService (SMTP)
├── Email/NullEmailService.cs       # NullObject para dev
├── Logging/ExceptionInfoExtractor.cs
└── Logging/ExceptionLogStorage.cs
```

Ejemplos:
- `Features/Event/Add/` → `AddEventHandler`, `AddEventRequest`, `AddEventValidator`
- `Persistence/Features/Event/` → `EventRepository`, `EventConfiguration`, `EventSeeder`
- `Features/Reservation/Cancel/` → `CancelReservationHandler`, `CancelReservationRequest`, `CancelReservationValidator`
- `Persistence/Features/Reservation/` → `ReservationRepository`, `ReservationConfiguration`
- `Features/User/Login/` → `LoginHandler`, `LoginRequest`, `LoginRequestValidator`
- `Persistence/Features/User/` → `UserRepository`, `UserConfiguration`, `UserSeeder`

Registro DI en `ApplicationDependencyInjections.cs`: `AddScoped<Handler>()` + `AddValidatorsFromAssembly()`.
Registro DI en `InfrastructureDependencyInjections.cs`: repositorios, `IEmailService`, `IJwtService`, `IPasswordHasher`, `IAlphaNumericCodeGenerator`, `IDateTimeProvider`, seeders.

---

## 6. Patrones y mecanismos core

| Patrón | Implementación concreta | Archivo |
|--------|------------------------|---------|
| **Result<T>** | Struct inmutable: `IsSuccess`, `Value`, `Errors: ImmutableArray<string>`. Factory methods `Success()`, `Failure()`. Implícito `T → Result<T>`. | `Application/Common/ResultPattern/Result.cs` |
| **BaseUseCase<TReq,TRes>** | Template method: `Execute()` → `ValidateAsync()` → `OnExecute()`. Inyecta `IValidator<TRequest>`. | `Application/Common/UseCases/BaseUseCase.cs` |
| **FluentValidation por feature** | Un `AbstractValidator<TRequest>` por Request, validaciones sincrónicas/asíncronas, mensajes desde `SystemMessages.Validations`. | `Features/*/Validator.cs` |
| **JWT + Revocación** | Claims: `jti` (Guid), `nameid` (UserId), `name` (Username), `role`. `Generate()` → token. `Revoke(jti, exp)` → `MemoryCache.Set(jti, true, AbsoluteExpiration=exp)`. `IsRevoked(jti)` → `MemoryCache.TryGetValue`. | `Infrastructure/Tools/JwtService.cs` |
| **DateTimeProvider** | `IDateTimeProvider` (Core) → `SystemDateTimeProvider` (Infrastructure). `GetNowColombia()` usa `TimeZoneInfo` Colombia. Inyectado en handlers para testabilidad. | `Core/Common/Time/IDateTimeProvider.cs` |
| **GlobalExceptionMiddleware** | Catch all → mapea exception a HTTP status → `Result.Failure(mensaje)` → log archivo diario `ExceptionLogs/Errors-yyyy-MM-dd.txt` (SemaphoreSlim). **Hardcoded**: ruta `ExceptionLogs/`, nombre `Errors-yyyy-MM-dd.txt`, extracción via `IExceptionInfoExtractor` (mensaje, stack trace, inner exceptions). | `Api/Middlewares/GlobalExceptionMiddleware.cs` |
| **BaseRepository<T>** | CRUD genérico + `BuildQuery(predicate, includes, noTracking)`. `AsNoTrackingWithIdentityResolution()` por defecto. | `Infrastructure/Persistence/Common/Repository/BaseRepository.cs` |
| **Seeders** | `MainSeeder` → `UserSeeder` (admin), `VenueSeeder` (venues por defecto). Ejecutado en `ApiBuilderExtensions.ApplyMigrationsAndSeedAsync()`. | `Infrastructure/Persistence/Common/DataSeed/` |
| **Generación códigos alfanuméricos (Base62)** | `IAlphaNumericCodeGenerator` → `AlphaNumericCodeGenerator`. Alfabeto `0-9A-Za-z` (62 chars), `RandomNumberGenerator.GetInt32` criptográfico. Formato `EV-` + 6 chars = ~56.8 bits entropía. Usado en `ConfirmReservationHandler` con reintento (máx 5) por colisión. | `Infrastructure/Tools/AlphaNumericCodeGenerator.cs` |

---

## 7. Validación y reglas de negocio

### 7.1 Validaciones de entrada (FluentValidation por feature)

| Feature | Validator | Reglas (campo → regla) |
|---------|-----------|------------------------|
| **Auth/Login** | `LoginRequestValidator` | `Username` requerido, `Password` requerido |
| **Auth/Logout** | `LogoutRequestValidator` | `Jti` requerido, `ExpiresDate` requerido |
| **Venue/Add** | `AddVenueValidator` | `Name` requerido + máx 32, `Capacity` > 0, `City` requerido + máx 32 |
| **Event/Add** | `AddEventValidator` | `Title` 5-100, `Description` 10-500, `VenueId` >0, `MaxCapacity` >0, `StartDate` requerido, `EndDate` requerido + > StartDate, `Price` >0, `EventType` enum válido |
| **Event/OccupationReport** | `GetOccupationReportValidator` | `EventId` > 0 |
| **Reservation/Add** | `AddReservationValidator` | `EventId` >0, `Quantity` ≥1, `BuyerName` 1-32, `BuyerEmail` email válido 1-64 |
| **Reservation/GetByCode** | `GetByReservationCodeValidator` | `BuyerEmail` email 1-64, `ReservationCode` requerido + formato `EV-XXXXXX` (BaseReservationCodeValidator) |
| **Reservation/Cancel** | `CancelReservationValidator` | `BuyerEmail` email 1-64, `ReservationCode` formato `EV-XXXXXX` |
| **Reservation/Confirm** | `ConfirmReservationValidator` | `ReservationId` >0 |

> Mensajes de error desde `SystemMessages.Validations`: `Error_Required`, `Error_MaxLength`, `Error_MinLength`, `Error_GreaterThan`, `Error_Email`, `Error_InvalidFormat`, `Error_NotFound`, `Error_CouldNotBeConverted`.

### 7.2 Reglas de negocio (tabla consolidada)

| Regla | Handler | Código error (`SystemMessages`) | HTTP | Implementación |
|-------|---------|-------------------------------|------|----------------|
| Venue existe | `AddEventHandler` | `Error_NotFound` (Venue) | 400 | `IVenueRepository.GetByIdAsync(VenueId)` |
| Capacidad evento ≤ capacidad venue | `AddEventHandler` | `Rule_EventCapacityLimit` | 400 | `request.MaxCapacity > venue.Capacity` |
| No solape de eventos en mismo venue | `AddEventHandler` | `Rule_EventScheduleOverlap` | 400 | `IEventRepository.HasOverlappingEventAsync(venueId, start, end, null)` |
| Evento existe y está activo | `AddReservationHandler` | `Error_NotFound` (Event) | 400 | `IEventRepository.GetByIdAsync(EventId)` + `IsActive` |
| Evento no iniciado | `AddReservationHandler` | `Rule_ReservationTooLate` | 400 | `StartDate - NowColombia < 0` |
| Evento inicia en ≥ 1 hora | `AddReservationHandler` | `Rule_ReservationTooCloseToStart` | 400 | `(StartDate - Now).TotalSeconds < 3600` |
| Capacidad disponible ≥ Quantity | `AddReservationHandler` | `Rule_InsufficientCapacity` (available) | 400 | `MaxCapacity - GetCurrentOccupationByEventIdAsync()` |
| Si <24h al inicio → máx 5 entradas | `AddReservationHandler` | `Rule_MaxQuantityForLastDay` | 400 | `secondsUntilStart < 86400 && Quantity > 5` |
| Si precio > $100 → máx 10 entradas | `AddReservationHandler` | `Rule_MaxQuantityForExpensiveEvent` | 400 | `Price > 100m && Quantity > 10` |
| Cancelar solo reservas Confirmadas | `CancelReservationHandler` | `Rule_OnlyConfirmedCanBeCancelled` | 400 | `reservation.Status != ReservationStatus.Confirmed` |
| Penalización si cancelación <48h | `CancelReservationHandler` | `Rule_CancellationPenalty` | 200 | `HasPenalty = (StartDate - Now).TotalHours < 48` |
| Código único al confirmar (max 5 intentos) | `ConfirmReservationHandler` | `Rule_UniqueCodeGenerationFailed` | 400 | Loop `IAlphaNumericCodeGenerator.Generate()` + `ReservationRepository.ExistsByCodeAsync()` |
| Usuario existe y credenciales válidas | `LoginHandler` | `Error_Credentials` | 400 | `IUserRepository.GetByUsernameAsync` + `PasswordHasher.Verify` |
| Revocación JTI en logout | `LogoutHandler` | `Error_RevokedToken` / `Error_InvalidToken` | 401 | `IJwtService.Revoke(jti, exp)` + `OnTokenValidated` valida `IsRevoked` |

---

## 8. Autenticación y autorización

**Flujo JWT:**
1. `POST /api/Auth/login` (público) → valida credenciales → `JwtService.Generate()` → token con claims: `jti` (Guid), `nameid` (UserId), `name` (Username), `role` (UserRole)
2. Cliente envía `Authorization: Bearer <token>`
3. Pipeline `JwtBearerEvents.OnTokenValidated`:
   - Extrae `jti` del token
   - `IJwtService.IsRevoked(jti)` → si true → `context.Fail(Error_RevokedToken)`
   - Valida lifetime (`ClockSkew=Zero`)
4. `OnChallenge` / `OnAuthenticationFailed` → respuesta 401 con `Result.Failure(Error_Unauthorized/Error_InvalidToken)` (JSON)
5. `POST /api/Auth/logout` (requiere JWT) → extrae `jti` + `exp` del token → `IJwtService.Revoke(jti, exp)` → 200

**Endpoints públicos:** `GET /api/Venues`, `GET /api/Event`, `POST /api/Auth/login`, `GET /api/Reservations/getByReservationCode`, `POST /api/Reservations`, `PUT /api/Reservations/cancel`

**Endpoints protegidos (JWT):** resto. Controlador base `[Authorize]` + `[AllowAnonymous]` selectivo.

---

## 9. Endpoints (referencia completa)

| Método | Ruta | Auth | Request DTO | Response DTO | Códigos HTTP | Reglas clave (ref 7.2) |
|--------|------|------|-------------|--------------|--------------|------------------------|
| POST | `/api/Auth/login` | Público | `LoginRequest` | `Result<string>` (JWT) | 200, 400 | Credenciales válidas |
| POST | `/api/Auth/logout` | JWT | — | 204 | 204, 401 | Revocación JTI |
| GET | `/api/Venues` | Público | — | `Result<List<VenueDto>>` | 200 | - |
| POST | `/api/Venues` | JWT | `AddVenueRequest` | `Result<VenueDto>` | 200, 400, 401 | Validaciones Venue |
| GET | `/api/Event` | Público | — | `Result<List<EventDTO>>` (con Venue) | 200 | - |
| GET | `/api/Event/occupationReport` | JWT | `EventId` (query) | `Result<EventOccupationReportDto>` | 200, 400, 401, 404 | Autorización |
| POST | `/api/Event` | JWT | `AddEventRequest` | `Result<EventDTO>` | 200, 400, 401 | Venue existe, capacidad, solape |
| GET | `/api/Reservations` | JWT | — | `Result<List<ReservationDTO>>` | 200, 401 | - |
| GET | `/api/Reservations/getByReservationCode` | Público | `BuyerEmail`, `ReservationCode` | `Result<ReservationDTO>` (con Event) | 200, 400, 404 | Formato código EV-XXXXXX |
| POST | `/api/Reservations` | Público | `AddReservationRequest` | `Result<ReservationDTO>` (PendientePago) | 200, 400 | 7 reglas negocio (7.2) |
| PUT | `/api/Reservations/cancel` | Público | `BuyerEmail`, `ReservationCode` | 204 | 204, 400, 404 | Solo confirmadas, penalización |
| PUT | `/api/Reservations/confirm` | JWT | `ReservationId` | `Result<string>` (código EV-XXXXXX) | 200, 400, 401, 404 | Código único max 5 intentos |

> Todas las respuestas usan envoltorio `Result<T>`: `{ "isSuccess": true, "value": {...}, "errors": [] }` o `{ "isSuccess": false, "value": null, "errors": ["mensaje"] }`.

---

## 10. Manejo de errores

**GlobalExceptionMiddleware** (`Api/Middlewares/GlobalExceptionMiddleware.cs`):
- Captura toda excepción no manejada
- Mapea a HTTP status:
  | Exception | HTTP | Mensaje (`SystemMessages`) |
  |-----------|------|----------------------------|
  | `ArgumentNullException`, `ArgumentException` | 400 | `Error_BadRequest` |
  | `UnauthorizedAccessException` | 401 | `Error_Unauthorized` |
  | `KeyNotFoundException` | 404 | `Error_NotFound` |
  | `InvalidOperationException` | 409 | `Error_Conflict` |
  | Otros | 500 | `Error_Internal` |
- **Extracción y almacenamiento** (`Infrastructure/Tools/Logging/`):
  - `IExceptionInfoExtractor.ExtractExceptionInfo(ex)` → string con mensaje, stack trace, inner exceptions recursivas
  - `IExceptionLogStorage.WriteAsync(content)` → escribe en `ExceptionLogs/Errors-yyyy-MM-dd.txt`
  - **Hardcoded**: directorio `ExceptionLogs/` (relativo a `Directory.GetCurrentDirectory()`), nombre `Errors-yyyy-MM-dd.txt`, formato texto plano, `SemaphoreSlim` para concurrencia
  - No rotación por tamaño, no compresión, no structured logging (JSON)
- Respuesta: `Result.Failure(mensaje)` serializado JSON (`application/json`)

**Validación FluentValidation** → 400 con `Result.Failure(errors[])` via `ApiBehaviorOptions.InvalidModelStateResponseFactory`.

**JWT Challenges** → 401 con `Result.Failure(Error_Unauthorized/Error_InvalidToken/Error_RevokedToken)` via `JwtBearerEvents`.

---