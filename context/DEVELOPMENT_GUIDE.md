# File-Explorer Backend - Guía de Desarrollo Rápida

## 🚀 Inicio Rápido

### Requisitos
- .NET 9.0 SDK
- Visual Studio Code o Visual Studio 2022
- SQLite (incluido en EF Core)

### Ejecutar el proyecto
```bash
cd src/Web
dotnet run
# API: https://localhost:5001
# Swagger: https://localhost:5001/
```

---

## 📝 Cómo Agregar una Nueva Funcionalidad

### Ejemplo: Crear un nuevo endpoint para obtener estadísticas de usuario

#### 1️⃣ **Domain Layer** - Definir la lógica pura

```csharp
// src/Domain/Entities/User.cs (agregar método)
public class User : Entity
{
    // ... propiedades existentes ...
    
    public UserStatistics GetStatistics()
    {
        return new UserStatistics
        {
            TotalFiles = 0, // Será calculado desde archivos
            UsedStorage = 0, // Será calculado desde archivos
            LastActivityDate = LastLoginAt
        };
    }
}

// src/Domain/ValueObjects/UserStatistics.cs (nuevo)
public record UserStatistics(
    int TotalFiles,
    long UsedStorage,
    DateTime? LastActivityDate
);
```

#### 2️⃣ **Application Layer** - Implementar el caso de uso

```csharp
// src/Application/Interfaces/IUserServices.cs (agregar interfaz)
public interface IUserServices
{
    // ... métodos existentes ...
    Task<ApiResult<UserStatistics>> GetUserStatisticsAsync(int userId);
}

// src/Application/Services/UserServices.cs (implementar)
public class UserServices : Service<User>, IUserServices
{
    // ... código existente ...
    
    public async Task<ApiResult<UserStatistics>> GetUserStatisticsAsync(int userId)
    {
        _logger.LogInformation("Fetching statistics for user {UserId}", userId);
        
        Maybe<User> maybeUser = await FindAsync(userId).ConfigureAwait(false);
        
        if (maybeUser.IsNone)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return ApiResult<UserStatistics>.Error("User not found", 404);
        }
        
        User user = maybeUser.Value;
        UserStatistics stats = user.GetStatistics();
        
        _logger.LogInformation("Statistics retrieved for user {UserId}", userId);
        return ApiResult<UserStatistics>.Success(stats, "Statistics retrieved", 200);
    }
}
```

#### 3️⃣ **Web Layer** - Exponer el endpoint

```csharp
// src/Web/Controllers/UserController.cs (agregar endpoint)
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IUserServices userServices, ILogger<UserController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }
    
    [HttpGet("{id}/statistics")]
    public async Task<IActionResult> GetStatistics(int id)
    {
        var result = await _userServices.GetUserStatisticsAsync(id).ConfigureAwait(false);
        
        if (result.Metadata?.StatusCode != 200)
            return StatusCode(result.Metadata?.StatusCode ?? 500, result);
        
        return Ok(result);
    }
}
```

---

## 🔄 Patrones Comunes

### Patrón: Buscar y Actualizar

```csharp
public async Task<ApiResult<bool>> UpdateUserEmailAsync(int userId, string newEmail)
{
    try
    {
        // 1. Validar entrada
        var emailResult = Email.Validate(newEmail);
        if (emailResult.IsFailure)
            return ApiResult<bool>.Error(emailResult.GetErrorOrThrow().Message, 400);
        
        // 2. Buscar entidad
        Maybe<User> maybeUser = await FindAsync(userId).ConfigureAwait(false);
        if (maybeUser.IsNone)
            return ApiResult<bool>.Error("User not found", 404);
        
        User user = maybeUser.Value;
        
        // 3. Verificar duplicados
        var existingUser = await FindByEmailAsync(newEmail).ConfigureAwait(false);
        if (existingUser.IsSome && existingUser.Value.Id != userId)
            return ApiResult<bool>.Error("Email already in use", 409);
        
        // 4. Actualizar
        user.Email = newEmail;
        user.UpdatedAt = DateTime.UtcNow;
        Update(user);
        
        // 5. Persistir
        await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);
        
        _logger.LogInformation("User {UserId} email updated to {Email}", userId, newEmail);
        return ApiResult<bool>.Success(true, "Email updated", 200);
    }
    catch (DbUpdateException dbEx)
    {
        _logger.LogError(dbEx, "Database error updating user email");
        return ApiResult<bool>.Error("Database error", 500);
    }
}
```

### Patrón: Validación con FluentValidation

```csharp
// src/Application/Validation/UpdateEmailRequestValidator.cs
public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequest>
{
    public UpdateEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");
        
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than zero");
    }
}

// Usar en servicio
var validationResult = await _validator.ValidateAsync(request);
if (!validationResult.IsValid)
{
    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
    return ApiResult<bool>.Error(errors, 400);
}
```

### Patrón: Mapeo con Mapster

```csharp
// src/Application/Mappings/UserMappingExtensions.cs
public static class UserMappingExtensions
{
    public static UserResponse ToDto(this User user)
    {
        return user.Adapt<UserResponse>();
    }
    
    public static List<UserResponse> ToDtos(this IEnumerable<User> users)
    {
        return users.Adapt<List<UserResponse>>();
    }
    
    public static User ToEntity(this CreateUserRequest request)
    {
        return request.Adapt<User>();
    }
}

// Usar en servicio
User user = request.Adapt<User>();
UserResponse dto = user.ToDto();
```

### Patrón: Transacciones

```csharp
public async Task<ApiResult<bool>> TransferFilesAsync(int fromUserId, int toUserId, List<Guid> fileIds)
{
    try
    {
        // Iniciar transacción
        _unitOfwork.BeginTransaction();
        
        // Operación 1: Validar usuarios
        var fromUser = await FindAsync(fromUserId);
        var toUser = await FindAsync(toUserId);
        
        if (fromUser.IsNone || toUser.IsNone)
        {
            _unitOfwork.Rollback();
            return ApiResult<bool>.Error("User not found", 404);
        }
        
        // Operación 2: Transferir archivos
        foreach (var fileId in fileIds)
        {
            // Lógica de transferencia
        }
        
        // Confirmar transacción
        _unitOfwork.Commit();
        
        return ApiResult<bool>.Success(true, "Files transferred", 200);
    }
    catch (Exception ex)
    {
        _unitOfwork.Rollback();
        _logger.LogError(ex, "Error transferring files");
        return ApiResult<bool>.Error("Transfer failed", 500);
    }
}
```

---

## 🗄️ Trabajar con la Base de Datos

### Crear una nueva migración

```bash
cd src/Infrastructure
dotnet ef migrations add AddNewFeature --project ../Infrastructure --startup-project ../Web
```

### Aplicar migraciones

```bash
cd src/Web
dotnet ef database update
```

### Ver migraciones pendientes

```bash
dotnet ef migrations list
```

### Revertir última migración

```bash
dotnet ef migrations remove
```

---

## 🧪 Testing - Ejemplos con Swagger

### 1. Registrar usuario
```
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "phone": "+34 123456789",
  "password": "SecurePass123!"
}
```

### 2. Login
```
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "SecurePass123!"
}
```

### 3. Usar token en requests autorizados
```
GET /api/v1/user/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 4. Refresh token
```
POST /api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "token_value_here"
}
```

---

## 📊 Estructura de Respuestas

### Respuesta exitosa
```json
{
  "data": {
    "email": "juan@example.com",
    "firstName": "Juan",
    "lastName": "Pérez",
    "phone": "+34 123456789",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "metadata": {
    "statusCode": 200,
    "message": "Login successful",
    "errors": null
  }
}
```

### Respuesta con error
```json
{
  "data": null,
  "metadata": {
    "statusCode": 400,
    "message": "Invalid credentials",
    "errors": null
  }
}
```

### Respuesta con errores de validación
```json
{
  "data": null,
  "metadata": {
    "statusCode": 400,
    "message": "Email is required, Password must be at least 8 characters",
    "errors": [
      "Email is required",
      "Password must be at least 8 characters"
    ]
  }
}
```

---

## 🔍 Debugging

### Habilitar logs detallados

```csharp
// src/Web/Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Cambiar a Debug
    .WriteTo.Console(
        outputTemplate: "[{RequestId}] {Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{RequestId}] {Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .Enrich.FromLogContext()
    .CreateLogger();
```

### Ver logs en tiempo real
```bash
tail -f src/Web/logs/log-20260227.txt
```

### Breakpoints en Visual Studio
1. Establecer breakpoint (F9)
2. Ejecutar en Debug (F5)
3. Inspeccionar variables (Hover)
4. Continuar (F10/F11)

---

## 🛡️ Seguridad - Checklist

- [ ] Contraseñas hasheadas con BCrypt
- [ ] JWT con expiración configurada
- [ ] HTTPS en producción
- [ ] CORS restringido a dominios permitidos
- [ ] Validación de entrada en todos los endpoints
- [ ] Manejo de excepciones sin exponer detalles internos
- [ ] Logs de auditoría para operaciones sensibles
- [ ] Rate limiting (pendiente)
- [ ] CSRF protection (pendiente)
- [ ] SQL injection prevention (EF Core protege)

---

## 📚 Recursos Útiles

### Documentación oficial
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Mapster](https://mapperly.riok.app/)

### Comandos útiles
```bash
# Limpiar build
dotnet clean

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Publicar
dotnet publish -c Release

# Ver versión de .NET
dotnet --version
```

---

## 🐛 Troubleshooting

### Error: "JwtSettings configuration is missing"
**Solución:** Verificar que `appsettings.json` contiene la sección `JwtSettings`

### Error: "Database connection failed"
**Solución:** Verificar que la ruta en `ConnectionStrings.DefaultConnection` es correcta

### Error: "Migration pending"
**Solución:** Ejecutar `dotnet ef database update`

### Error: "Token validation failed"
**Solución:** Verificar que el token no ha expirado y que la firma es válida

### Error: "CORS policy blocked"
**Solución:** Verificar que el origen está permitido en `AddCors()`

