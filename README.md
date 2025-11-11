<h1 align="center">File-Explorer 📂</h1>

<p align="center">
  <strong>Un explorador de archivos simple y potente, desarrollado para ofrecer una gestión de archivos y directorios intuitiva y eficiente.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Frontend-Angular-red" alt="Frontend Angular">
  <img src="https://img.shields.io/badge/Backend-.NET%20Core-blueviolet" alt="Backend .NET Core">
</p>

## Tabla de Contenidos

*   [Descripción General](#descripción-general)
*   [Características Principales](#características-principales)
*   [Vistazo a la Aplicación](#vistazo-a-la-aplicación)
*   [Tecnologías Utilizadas](#tecnologías-utilizadas)
*   [Arquitectura del Proyecto](#arquitectura-del-proyecto)
*   [Patrones de Diseño Utilizados](#patrones-de-diseño-utilizados)
*   [Prerrequisitos](#prerrequisitos)
*   [Instalación](#instalación)
*   [Uso](#uso)
*   [Roadmap (Funcionalidades Futuras)](#roadmap-funcionalidades-futuras)
*   [Contribuciones](#contribuciones)
*   [Licencia](#licencia)
*   [Contacto](#contacto)

## Descripción General

File-Explorer es una aplicación de escritorio moderna que permite a los usuarios navegar por el sistema de archivos, visualizar el contenido de los directorios y realizar operaciones esenciales con archivos y carpetas. La interfaz de usuario está construida con Angular, proporcionando una experiencia fluida y reactiva, mientras que la lógica del backend se maneja con .NET Core, asegurando robustez y rendimiento.

## Características Principales

*   ✅ **Navegación Intuitiva:** Explore su sistema de archivos con una estructura de árbol de directorios clara y fácil de usar.
*   ✅ **Visualización Detallada:** Vea el contenido de las carpetas, incluyendo detalles de archivos y previsualizaciones (si aplica).
*   ⏳ **Operaciones Básicas de Archivos:**
    *   Copiar archivos/carpetas
    *   Pegar archivos/carpetas
    *   Renombrar archivos/carpetas
    *   Eliminar archivos/carpetas
    *   Crear nuevas carpetas
    *   Búsqueda de archivos
    *   *(Más características se añadirán progresivamente)*

## Vistazo a la Aplicación

<p align="center">
  <img src="image/UIV2.png" alt="UI File-Explorer" width="60%">
  <br>
  <em>Navegación de carpetas en File-Explorer.</em>
</p>

<p align="center">
  <img src="image/Swagger.png" alt="Swagger API" width="60%">
  <br>
  <em>Documentación de la API con Swagger.</em>
</p>

## Tecnologías Utilizadas

*   **Frontend:** [Angular](https://angular.io/)
*   **Backend:** [.NET Core](https://dotnet.microsoft.com/)

## Arquitectura del Proyecto

El proyecto sigue una arquitectura por capas (Layered Architecture), comúnmente asociada con los principios de la Arquitectura Limpia (Clean Architecture). Esta separación de responsabilidades se evidencia en la estructura del proyecto con las siguientes capas principales:

*   **Web (Capa de Presentación):** Responsable de manejar las solicitudes HTTP, la interfaz de usuario (frontend con Angular) y la API (backend con .NET Core).
*   **Application (Capa de Aplicación):** Contiene la lógica de negocio y los casos de uso de la aplicación. Orquesta las interacciones entre la capa de presentación y la capa de infraestructura.
*   **Infrastructure (Capa de Infraestructura):** Se encarga de las implementaciones concretas de las abstracciones definidas en la capa de aplicación, como el acceso a datos, servicios externos, etc.
*   **Domain (Capa de Dominio):** Contiene las entidades y las reglas de negocio de la aplicación.

## Patrones de Diseño Utilizados

### Patrones Arquitectónicos

#### **Clean Architecture (Arquitectura Limpia)**
Separación en 4 capas independientes con dependencias unidireccionales hacia el dominio:
- **Domain:** Entidades, interfaces y reglas de negocio
- **Application:** Casos de uso, DTOs, servicios y validaciones
- **Infrastructure:** Implementación de repositorios, acceso a datos (EF Core)
- **Web:** Controllers, middleware, configuración de API

#### **Layered Architecture (Arquitectura por Capas)**
Organización jerárquica donde cada capa solo conoce la capa inmediatamente inferior.

### Patrones de Diseño Implementados

#### **1. Repository Pattern**
Abstrae el acceso a datos y proporciona una interfaz uniforme para operaciones CRUD.
```csharp
// Infrastructure/Repositories/Repository.cs
public interface IRepositoryAsync<T> where T : Entity
{
    Task<T?> GetByIdAsync(int id);
    IQueryable<T> Queryable();
}
```

#### **2. Unit of Work Pattern**
Coordina transacciones y cambios en múltiples repositorios.
```csharp
// Infrastructure/UnitOfWork.cs
public interface IUnitOfWorkAsync
{
    Task<int> SaveChangesAsync();
}
```

#### **3. Dependency Injection (DI)**
Inyección de dependencias en toda la aplicación para desacoplamiento y testabilidad.
```csharp
// Program.cs
services.AddScoped<IUserServices, UserServices>();
```

#### **4. Maybe Monad (Option Pattern)**
Manejo explícito de valores opcionales sin usar `null`.
```csharp
// Domain/Monads/MaybeT.cs
public async Task<Maybe<User>> FindByEmailAsync(string email)
{
    var user = await Queryable().FirstOrDefaultAsync(u => u.Email == email);
    return Maybe.From(user);
}
```

#### **5. Result Pattern**
Encapsula el resultado de operaciones que pueden fallar, propagando errores de forma funcional.
```csharp
// Domain/Monads/Result/Result.cs
public Result<Unit> ValidateEmail(string email)
{
    if (string.IsNullOrWhiteSpace(email))
        return Result.Failure<Unit>("Invalid email");
    return Result.Unit;
}
```

#### **6. Railway-Oriented Programming**
Encadenamiento de operaciones con manejo automático de errores usando `Bind` y `Map`.
```csharp
var result = ValidateEmail(email)
    .Bind(() => ValidatePassword(password))
    .Map(_ => new CreateUserRequest(email, password));
```

#### **7. Factory Method Pattern**
Creación de instancias a través de métodos estáticos.
```csharp
// Domain/Monads/Maybe.cs
public static Maybe<T> From<T>(T? value) => new(value);
public static Maybe<T> Some<T>(T value) => new(value);
```

#### **8. Service Layer Pattern**
Encapsula la lógica de negocio en servicios reutilizables.
```csharp
// Application/Services/UserServices.cs
public class UserServices : Service<User>, IUserServices
```

#### **9. DTO Pattern (Data Transfer Object)**
Objetos para transferir datos entre capas sin exponer entidades del dominio.
```csharp
// Application/DTOs/Request/CreateUserRequest.cs
// Application/DTOs/Response/LoginResponse.cs
```

#### **10. Mapper Pattern (con Mapster)**
Transformación automática entre entidades y DTOs.
```csharp
var dto = users.Adapt<List<GetUserResponse>>();
```

#### **11. Fluent Validation Pattern**
Validaciones declarativas y reutilizables.
```csharp
// Application/Validation/CreateUserRequestValidator.cs
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
```

#### **12. Middleware Pattern**
Procesamiento de requests HTTP en cadena.
```csharp
// Web/Middleware/RequestIdMiddleware.cs
app.UseMiddleware<RequestIdMiddleware>();
```

#### **13. API Result Pattern**
Respuestas HTTP estandarizadas con metadata.
```csharp
return ApiResult<LoginResponse>.Success(response, "Login successful", 200);
return ApiResult<LoginResponse>.Error("Invalid credentials", 401);
```

#### **14. Generic Repository Pattern**
Repositorio genérico para operaciones comunes en todas las entidades.
```csharp
public class Repository<T> : IRepositoryAsync<T> where T : Entity
```

#### **15. Options Pattern**
Configuración tipada y validada.
```csharp
// Application/Configuration/JwtSettings.cs
private readonly LockoutOptions _lockoutOptions = new();
```

### Patrones Funcionales

- **Immutability:** Uso de `readonly struct` en `Maybe<T>` y `Result<T>`
- **Pure Functions:** Métodos sin efectos secundarios en validaciones
- **Higher-Order Functions:** `Map`, `Bind`, `Match` en monads
- **Function Composition:** Encadenamiento de operaciones con `Bind`

### Principios SOLID Aplicados

- **S** - Single Responsibility: Cada servicio tiene una responsabilidad única
- **O** - Open/Closed: Extensible mediante interfaces
- **L** - Liskov Substitution: Implementaciones intercambiables
- **I** - Interface Segregation: Interfaces específicas por funcionalidad
- **D** - Dependency Inversion: Dependencias hacia abstracciones

---

## Roadmap (Funcionalidades Futuras)

- [ ] Token Refresh (JWT)
- [x] Bloqueo de cuenta por intentos fallidos
- [ ] Autenticación con Google OAuth
- [ ] Recuperación de contraseña
- [ ] Búsqueda avanzada de archivos
- [ ] Sistema de permisos y roles
- [ ] Compartir archivos con enlaces 