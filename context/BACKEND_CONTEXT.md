# File-Explorer Backend - Contexto Técnico

## 📋 Resumen Ejecutivo

**File-Explorer** es una plataforma web de gestión de archivos corporativos construida con:
- **Backend:** .NET Core 9.0 (C#)
- **Frontend:** Angular 19
- **Base de Datos:** SQLite
- **Arquitectura:** Clean Architecture (4 capas)

---

## 🏗️ Arquitectura de Capas

### 1. **Domain** (`src/Domain/`)
Capa de dominio puro sin dependencias externas.

**Entidades principales:**
- `User` - Usuarios del sistema con autenticación y cuotas de almacenamiento
- `FileSystemItem` (abstracta) - Base para archivos y directorios
  - `FileItem` - Archivos individuales
  - `DirectoryItem` - Directorios/carpetas
- `RefreshToken` - Tokens de refresco JWT

**Monads (Programación Funcional):**
- `Result<T>` - Manejo de errores sin excepciones
- `Maybe<T>` - Valores opcionales seguros
- `Errors` - Tipos de errores del dominio

**Value Objects:**
- `Email`, `FirstName`, `LastName`, `Password` - Validación de datos

### 2. **Application** (`src/Application/`)
Lógica de negocio y casos de uso.

**Servicios principales:**
- `UserServices` - Autenticación, registro, gestión de perfiles
- `FileServices` - Operaciones con archivos
- `FolderServices` - Operaciones con directorios
- `JwtTokenService` - Generación y validación de tokens JWT

**DTOs:**
- Request: `CreateUserRequest`, `LoginRequest`, `EditUserRequest`
- Response: `LoginResponse`, `GetUserResponse`, `ApiResult<T>`

**Validación:**
- FluentValidation para validaciones declarativas
- `CreateUserRequestValidator`

**Mapeo:**
- Mapster para transformación Entity ↔ DTO

### 3. **Infrastructure** (`src/Infrastructure/`)
Acceso a datos y servicios externos.

**DbContext:**
- `FileExplorerDbContext` - Contexto EF Core con SQLite

**Repositorios:**
- `Repository<T>` - Repositorio genérico
- `FileRepository` - Operaciones específicas de archivos
- `FolderRepository` - Operaciones específicas de carpetas
- `RefreshTokenRepository` - Gestión de tokens

**Unit of Work:**
- `IUnitOfWork` / `IUnitOfWorkAsync` - Transaccionalidad
- `UnitOfWork` - Implementación con gestión de repositorios

**Migraciones:**
- EF Core Migrations para versionado de BD

### 4. **Web** (`src/Web/`)
Presentación y API REST.

**Controladores:**
- `AuthController` - Login, registro, refresh token, reset password
- `FileController` - CRUD de archivos, descarga, compartir
- `FolderController` - CRUD de carpetas
- `UserController` - Gestión de usuarios
- `SearchController` - Búsqueda de archivos
- `PermissionController` - Control de acceso

**Middleware:**
- `RequestIdMiddleware` - Trazabilidad de requests

**Configuración:**
- JWT Bearer authentication
- CORS habilitado
- Swagger/OpenAPI documentación
- Serilog para logging

---

## 🔐 Autenticación y Autorización

### Flujo de Autenticación:
1. **Login** → Validar credenciales → Generar JWT + Refresh Token
2. **Refresh Token** → Validar token expirado → Generar nuevo JWT
3. **Logout** → Revocar refresh token

### Seguridad:
- Contraseñas hasheadas con BCrypt
- JWT con expiración configurable (24h por defecto)
- Bloqueo de cuenta tras 5 intentos fallidos (5 minutos)
- Tokens de reset de contraseña con expiración (1 hora)

### Configuración JWT (`appsettings.json`):
```json
{
  "JwtSettings": {
    "SecretKey": "7K9mP2nQ5rT8wX1zA4bC6dF0gH3jL5mN8pR1sU4vY7zA2bD5eG8hK0mP3qS6tW9x",
    "Issuer": "FileExplorer",
    "Audience": "FileExplorerUsers",
    "ExpirationHours": 24
  }
}
```

---

## 📊 Modelo de Datos

### User
```
Id (PK)
FirstName, LastName, Email (UNIQUE), Phone
PasswordHash
FailedLoginAttempts, LockoutEnd
StorageQuotaBytes (5GB default)
PasswordResetToken, PasswordResetTokenExpiry
CreatedAt, UpdatedAt, IsActive, LastLoginAt
```

### RefreshToken
```
Id (PK)
Token (UNIQUE), UserId (FK)
Created, Expire
```

### FileSystemItem (Jerarquía)
```
FileItem
├─ Name, Path, Size
├─ CreatedAt, ModifiedAt
└─ ItemType = File

DirectoryItem
├─ Name, Path, Size
├─ CreatedAt, ModifiedAt
├─ Files (ICollection<FileItem>)
├─ SubFolders (ICollection<DirectoryItem>)
└─ ItemType = Directory
```

---

## 🔄 Patrones de Diseño Implementados

### Arquitectónicos:
- **Clean Architecture** - Separación clara de responsabilidades
- **Layered Architecture** - 4 capas independientes

### Estructurales:
- **Repository Pattern** - Abstracción de persistencia
- **Unit of Work** - Transaccionalidad
- **Dependency Injection** - IoC container
- **Service Layer** - Encapsulamiento de lógica

### Creacionales:
- **Factory Method** - Creación de repositorios dinámicos
- **Options Pattern** - Configuración tipada

### Funcionales:
- **Result Pattern** - Manejo de errores sin excepciones
- **Maybe Monad** - Valores opcionales seguros
- **Railway-Oriented Programming** - Encadenamiento de operaciones

---

## 📦 Dependencias Principales

```xml
<!-- Backend -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="BCrypt.Net-Next" />
<PackageReference Include="FluentValidation.AspNetCore" />
<PackageReference Include="Mapster" />
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="Swashbuckle.AspNetCore" />
<PackageReference Include="TrackableEntities.EF.Core" />
```

---

## 🚀 Endpoints Principales

### Autenticación
- `POST /api/v1/auth/login` - Login
- `POST /api/v1/auth/register` - Registro
- `POST /api/v1/auth/refresh` - Refresh token
- `POST /api/v1/auth/logout` - Logout
- `POST /api/v1/auth/forgot-password` - Solicitar reset
- `POST /api/v1/auth/reset-password` - Resetear contraseña

### Usuarios
- `GET /api/v1/user/{id}` - Obtener usuario
- `PUT /api/v1/auth/editUser` - Actualizar perfil

### Archivos
- `GET /api/v1/file/{id}` - Obtener archivo
- `POST /api/v1/file/upload` - Subir archivo
- `GET /api/v1/file/{id}/download` - Descargar archivo
- `DELETE /api/v1/file/{id}` - Eliminar archivo
- `POST /api/v1/file/{id}/share` - Compartir archivo

---

## 🔧 Configuración y Ejecución

### Requisitos:
- .NET 9.0 SDK
- SQLite (incluido en EF Core)

### Estructura de carpetas:
```
File-Explorer-2/
├── src/
│   ├── Domain/              # Entidades y lógica pura
│   ├── Application/         # Servicios y casos de uso
│   ├── Infrastructure/      # Acceso a datos
│   └── Web/                 # API REST y Angular
├── CONTENEDOR/              # Almacenamiento de archivos
└── fileexplorer.db          # Base de datos SQLite
```

### Ejecución:
```bash
cd src/Web
dotnet run
# API disponible en: https://localhost:5001
# Swagger en: https://localhost:5001/
```

---

## 📝 Logging

**Serilog** configurado con:
- Console output
- File rolling (diario)
- Seq integration (opcional)
- RequestId en cada log para trazabilidad

Logs guardados en: `src/Web/logs/log-YYYYMMDD.txt`

---

## 🎯 Roadmap Pendiente

- [ ] Token Refresh ininterrumpido
- [ ] Google OAuth (SSO)
- [ ] Búsqueda avanzada con filtros
- [ ] RBAC granular
- [ ] Enlaces públicos con expiración
- [ ] Auditoría completa de operaciones

---

## 📚 Recursos Clave

- **Program.cs** - Configuración de servicios y middleware
- **FileExplorerDbContext.cs** - Mapeo de entidades
- **UserServices.cs** - Lógica de autenticación
- **JwtTokenService.cs** - Generación de tokens
- **UnitOfWork.cs** - Gestión transaccional
- **appsettings.json** - Configuración de aplicación

