# 📚 File-Explorer Backend - Documentación Completa

## 🎯 Índice de Documentación

Este proyecto incluye documentación completa para facilitar el desarrollo y mantenimiento del backend.

### 📖 Documentos Disponibles

1. **[BACKEND_CONTEXT.md](./BACKEND_CONTEXT.md)** - Contexto Técnico General
   - Resumen ejecutivo del proyecto
   - Descripción de las 4 capas de arquitectura
   - Modelo de datos
   - Patrones de diseño implementados
   - Dependencias principales
   - Endpoints principales
   - Configuración y ejecución

2. **[ARCHITECTURE_DIAGRAMS.md](./ARCHITECTURE_DIAGRAMS.md)** - Diagramas de Arquitectura
   - Flujo de autenticación
   - Estructura de capas
   - Flujo de seguridad
   - Modelo de datos (ERD)
   - Ciclo de vida de una solicitud
   - Inyección de dependencias
   - Ejemplo: Flujo completo de registro

3. **[DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)** - Guía de Desarrollo
   - Inicio rápido
   - Cómo agregar nueva funcionalidad (paso a paso)
   - Patrones comunes
   - Trabajar con la base de datos
   - Testing con Swagger
   - Estructura de respuestas
   - Debugging
   - Checklist de seguridad
   - Troubleshooting

4. **[API_REFERENCE.md](./API_REFERENCE.md)** - Referencia de APIs
   - Endpoints de autenticación
   - Endpoints de usuarios
   - Endpoints de archivos
   - Endpoints de carpetas
   - Endpoints de búsqueda
   - Endpoints de permisos
   - Health check
   - Códigos de estado HTTP
   - Headers requeridos
   - Estructura de errores
   - Ejemplos de uso

---

## 🚀 Inicio Rápido

### Para nuevos desarrolladores:

1. **Leer primero:** [BACKEND_CONTEXT.md](./BACKEND_CONTEXT.md)
   - Entender la arquitectura general
   - Familiarizarse con las 4 capas

2. **Visualizar:** [ARCHITECTURE_DIAGRAMS.md](./ARCHITECTURE_DIAGRAMS.md)
   - Ver cómo interactúan los componentes
   - Entender el flujo de datos

3. **Desarrollar:** [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)
   - Aprender patrones comunes
   - Seguir ejemplos paso a paso

4. **Consultar:** [API_REFERENCE.md](./API_REFERENCE.md)
   - Referencia rápida de endpoints
   - Ejemplos de requests/responses

---

## 📁 Estructura del Proyecto

```
File-Explorer-2/
├── src/
│   ├── Domain/                          # Capa de dominio
│   │   ├── Entities/                    # Entidades del negocio
│   │   │   ├── User.cs
│   │   │   ├── FileSystemItem.cs
│   │   │   ├── FileItem.cs
│   │   │   ├── DirectoryItem.cs
│   │   │   └── RefreshToken.cs
│   │   ├── Interfaces/                  # Contratos de dominio
│   │   ├── Monads/                      # Programación funcional
│   │   │   ├── Result/
│   │   │   ├── Maybe.cs
│   │   │   └── Errors/
│   │   └── ValueObjects/                # Objetos de valor
│   │       ├── Email.cs
│   │       ├── Password.cs
│   │       └── ...
│   │
│   ├── Application/                     # Capa de aplicación
│   │   ├── Services/                    # Servicios de negocio
│   │   │   ├── UserServices.cs
│   │   │   ├── FileServices.cs
│   │   │   ├── FolderServices.cs
│   │   │   └── JwtTokenService.cs
│   │   ├── Interfaces/                  # Contratos de servicios
│   │   ├── DTOs/                        # Objetos de transferencia
│   │   │   ├── Request/
│   │   │   └── Response/
│   │   ├── Validation/                  # Validadores
│   │   ├── Mappings/                    # Mapeos Entity ↔ DTO
│   │   └── Configuration/               # Configuraciones
│   │
│   ├── Infrastructure/                  # Capa de infraestructura
│   │   ├── Data/                        # DbContext
│   │   │   └── FileExplorerDbContext.cs
│   │   ├── Repositories/                # Repositorios
│   │   │   ├── Repository.cs
│   │   │   ├── FileRepository.cs
│   │   │   ├── FolderRepository.cs
│   │   │   └── RefreshTokenRepository.cs
│   │   ├── Interfaces/                  # Contratos de repositorios
│   │   ├── Migrations/                  # Migraciones EF Core
│   │   ├── UnitOfWork.cs                # Patrón Unit of Work
│   │   └── DependencyInjection.cs       # Inyección de dependencias
│   │
│   └── Web/                             # Capa de presentación
│       ├── Controllers/                 # Controladores API
│       │   ├── AuthController.cs
│       │   ├── FileController.cs
│       │   ├── FolderController.cs
│       │   ├── UserController.cs
│       │   ├── SearchController.cs
│       │   └── PermissionController.cs
│       ├── Middleware/                  # Middleware personalizado
│       ├── ClientApp/                   # Frontend Angular
│       ├── Program.cs                   # Configuración principal
│       ├── appsettings.json             # Configuración
│       └── Web.csproj
│
├── CONTENEDOR/                          # Almacenamiento de archivos
├── fileexplorer.db                      # Base de datos SQLite
├── BACKEND_CONTEXT.md                   # Este documento
├── ARCHITECTURE_DIAGRAMS.md
├── DEVELOPMENT_GUIDE.md
├── API_REFERENCE.md
└── README.md
```

---

## 🔑 Conceptos Clave

### Clean Architecture
El proyecto sigue los principios de Clean Architecture con 4 capas independientes:
- **Domain:** Lógica pura sin dependencias
- **Application:** Casos de uso y servicios
- **Infrastructure:** Acceso a datos
- **Web:** Presentación y API

### Patrones Implementados
- **Repository Pattern:** Abstracción de persistencia
- **Unit of Work:** Transaccionalidad
- **Dependency Injection:** IoC container
- **Result Pattern:** Manejo de errores sin excepciones
- **Maybe Monad:** Valores opcionales seguros
- **Railway-Oriented Programming:** Encadenamiento seguro

### Seguridad
- Contraseñas hasheadas con BCrypt
- JWT con expiración configurable
- Bloqueo de cuenta tras intentos fallidos
- Validación de entrada en todos los endpoints
- CORS configurado
- Logging de auditoría

---

## 🛠️ Stack Tecnológico

### Backend
- **.NET 9.0** - Framework principal
- **C#** - Lenguaje de programación
- **Entity Framework Core** - ORM
- **SQLite** - Base de datos
- **JWT** - Autenticación
- **BCrypt** - Hashing de contraseñas
- **FluentValidation** - Validación
- **Mapster** - Mapeo de objetos
- **Serilog** - Logging
- **Swagger/OpenAPI** - Documentación

### Frontend
- **Angular 19** - Framework
- **TypeScript** - Lenguaje
- **RxJS** - Programación reactiva

---

## 📊 Flujos Principales

### 1. Autenticación (Login)
```
Cliente → AuthController.Login()
        → UserServices.AuthenticateUserAsync()
        → Repository<User>.FindAsync()
        → Validar contraseña (BCrypt)
        → Generar JWT
        → UnitOfWork.SaveChangesAsync()
        → Retornar LoginResponse
```

### 2. Crear Archivo
```
Cliente → FileController.UploadFile()
        → FileServices.UploadFileAsync()
        → Validar usuario y cuota
        → Guardar archivo en disco
        → Repository<FileItem>.Insert()
        → UnitOfWork.SaveChangesAsync()
        → Retornar FileResponse
```

### 3. Buscar Archivos
```
Cliente → SearchController.Search()
        → SearchServices.SearchAsync()
        → Repository<FileItem>.Queryable()
        → Aplicar filtros
        → Retornar resultados
```

---

## 🔐 Seguridad

### Autenticación
- Login con email/contraseña
- Generación de JWT (24h expiración)
- Refresh token para renovación
- Bloqueo de cuenta tras 5 intentos fallidos

### Autorización
- Validación de JWT en cada request
- Claims: UserId, Email, Role
- Atributo [Authorize] en controladores

### Validación
- FluentValidation en DTOs
- Value Objects para datos críticos
- Sanitización de entrada

### Logging
- Serilog con múltiples sinks
- RequestId para trazabilidad
- Logs de operaciones sensibles

---

## 📈 Escalabilidad

### Consideraciones
- Repositorio genérico para reutilización
- Unit of Work para transacciones
- Async/await en todas las operaciones
- Inyección de dependencias para testabilidad
- Separación clara de responsabilidades

### Mejoras Futuras
- Caché distribuido (Redis)
- Message queue (RabbitMQ)
- Búsqueda avanzada (Elasticsearch)
- Replicación de BD
- Load balancing

---

## 🧪 Testing

### Unitarios
```csharp
[Test]
public async Task AuthenticateUserAsync_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var userService = new UserServices(...);
    
    // Act
    var result = await userService.AuthenticateUserAsync("user@example.com", "password");
    
    // Assert
    Assert.That(result.Metadata.StatusCode, Is.EqualTo(200));
    Assert.That(result.Data.Token, Is.Not.Null);
}
```

### Integración
```bash
# Usar Swagger para probar endpoints
https://localhost:5001/

# O usar curl
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'
```

---

## 📝 Convenciones de Código

### Nomenclatura
- **Clases:** PascalCase (UserService)
- **Métodos:** PascalCase (GetUserAsync)
- **Variables:** camelCase (userName)
- **Constantes:** UPPER_SNAKE_CASE (MAX_ATTEMPTS)
- **Interfaces:** IPrefixPascalCase (IUserService)

### Estructura de Métodos
```csharp
public async Task<ApiResult<T>> MethodNameAsync(params)
{
    try
    {
        // 1. Validar entrada
        // 2. Buscar/Procesar datos
        // 3. Aplicar lógica de negocio
        // 4. Persistir cambios
        // 5. Retornar resultado
        
        return ApiResult<T>.Success(data, "Message", 200);
    }
    catch (DbUpdateException dbEx)
    {
        _logger.LogError(dbEx, "Database error");
        return ApiResult<T>.Error("Error message", 500);
    }
}
```

---

## 🐛 Debugging

### Logs
```bash
# Ver logs en tiempo real
tail -f src/Web/logs/log-20260227.txt

# Buscar errores
grep "ERROR" src/Web/logs/log-*.txt
```

### Breakpoints
1. Establecer en Visual Studio (F9)
2. Ejecutar en Debug (F5)
3. Inspeccionar variables (Hover)
4. Continuar (F10/F11)

### Swagger
```
https://localhost:5001/
```

---

## 📞 Soporte

### Recursos
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)

### Contacto
- Revisar logs en `src/Web/logs/`
- Usar Swagger para probar endpoints
- Consultar documentación en este repositorio

---

## 📋 Checklist de Desarrollo

### Antes de hacer commit
- [ ] Código compila sin errores
- [ ] Tests pasan
- [ ] Logs no muestran errores
- [ ] Validación de entrada implementada
- [ ] Manejo de excepciones correcto
- [ ] Documentación actualizada
- [ ] Seguir convenciones de código

### Antes de hacer deploy
- [ ] Todas las migraciones aplicadas
- [ ] Configuración de producción correcta
- [ ] HTTPS habilitado
- [ ] CORS restringido
- [ ] Logging configurado
- [ ] Backups de BD
- [ ] Plan de rollback

---

## 🎓 Recursos de Aprendizaje

### Arquitectura
- Clean Architecture - Robert C. Martin
- Domain-Driven Design - Eric Evans
- Patterns of Enterprise Application Architecture - Martin Fowler

### C# y .NET
- C# Player's Guide - RB Whitaker
- CLR via C# - Jeffrey Richter
- Async/Await Best Practices - Stephen Cleary

### Patrones
- Design Patterns - Gang of Four
- Enterprise Integration Patterns - Gregor Hohpe
- Microservices Patterns - Chris Richardson

---

## 📅 Roadmap

### Completado ✅
- [x] Autenticación con JWT
- [x] Bloqueo de cuenta por intentos fallidos
- [x] Reset de contraseña
- [x] CRUD de usuarios
- [x] CRUD de archivos
- [x] CRUD de carpetas
- [x] Búsqueda básica
- [x] Compartir archivos

### En Progreso 🔄
- [ ] Token Refresh ininterrumpido
- [ ] Google OAuth (SSO)
- [ ] Búsqueda avanzada con filtros
- [ ] RBAC granular

### Pendiente 📋
- [ ] Auditoría completa
- [ ] Notificaciones en tiempo real
- [ ] Integración con servicios externos
- [ ] Análisis de almacenamiento
- [ ] Políticas de retención

---

## 📞 Preguntas Frecuentes

**P: ¿Cómo agregar un nuevo endpoint?**
R: Ver [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Sección "Cómo Agregar una Nueva Funcionalidad"

**P: ¿Cómo crear una migración?**
R: Ver [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Sección "Trabajar con la Base de Datos"

**P: ¿Cuál es la estructura de respuestas?**
R: Ver [API_REFERENCE.md](./API_REFERENCE.md) - Sección "Estructura de Errores"

**P: ¿Cómo debuggear un problema?**
R: Ver [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Sección "Debugging"

**P: ¿Qué patrones se usan?**
R: Ver [BACKEND_CONTEXT.md](./BACKEND_CONTEXT.md) - Sección "Estándares y Patrones de Diseño"

---

## 📄 Licencia

Este proyecto es parte de File-Explorer, una plataforma de gestión de archivos corporativos.

---

**Última actualización:** 27 de Febrero de 2025
**Versión:** 1.0.0
**Estado:** En desarrollo activo

