# File-Explorer Backend - Diagrama de Arquitectura

## 🔄 Flujo de Autenticación

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENTE (Angular)                         │
└────────────────────────────┬────────────────────────────────────┘
                             │
                    POST /api/v1/auth/login
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    WEB LAYER (Controllers)                       │
│                    AuthController.Login()                        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                 APPLICATION LAYER (Services)                     │
│              UserServices.AuthenticateUserAsync()                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ 1. Buscar usuario por email                              │   │
│  │ 2. Validar contraseña (BCrypt)                           │   │
│  │ 3. Verificar bloqueo de cuenta                           │   │
│  │ 4. Generar JWT token                                     │   │
│  │ 5. Actualizar LastLoginAt                                │   │
│  └──────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              INFRASTRUCTURE LAYER (Repositories)                 │
│                  Repository<User>.FindAsync()                    │
│                  UnitOfWork.SaveChangesAsync()                   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DATA LAYER (DbContext)                        │
│              FileExplorerDbContext (SQLite)                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DATABASE (SQLite)                             │
│                    fileexplorer.db                               │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📦 Estructura de Capas

```
┌─────────────────────────────────────────────────────────────────┐
│                         WEB LAYER                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Controllers:                                             │   │
│  │ • AuthController      (Login, Register, Refresh)        │   │
│  │ • FileController      (Upload, Download, Delete)        │   │
│  │ • FolderController    (CRUD de carpetas)                │   │
│  │ • UserController      (Gestión de usuarios)             │   │
│  │ • SearchController    (Búsqueda)                        │   │
│  │ • PermissionController (Control de acceso)              │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Middleware:                                              │   │
│  │ • RequestIdMiddleware (Trazabilidad)                    │   │
│  │ • JWT Authentication                                    │   │
│  │ • CORS                                                  │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             ▲
                             │ Inyección de Dependencias
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Services:                                                │   │
│  │ • UserServices        (Autenticación, Perfiles)         │   │
│  │ • FileServices        (Operaciones de archivos)         │   │
│  │ • FolderServices      (Operaciones de carpetas)         │   │
│  │ • JwtTokenService     (Generación de tokens)            │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ DTOs:                                                    │   │
│  │ • CreateUserRequest, LoginRequest, EditUserRequest      │   │
│  │ • LoginResponse, GetUserResponse, ApiResult<T>          │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Validación:                                              │   │
│  │ • FluentValidation (CreateUserRequestValidator)         │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Mapeo:                                                   │   │
│  │ • Mapster (Entity ↔ DTO)                                │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             ▲
                             │ Interfaces
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Repositorios:                                            │   │
│  │ • Repository<T>       (Genérico)                        │   │
│  │ • FileRepository      (Específico de archivos)          │   │
│  │ • FolderRepository    (Específico de carpetas)          │   │
│  │ • RefreshTokenRepository (Gestión de tokens)            │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Unit of Work:                                            │   │
│  │ • IUnitOfWork / IUnitOfWorkAsync                        │   │
│  │ • Gestión transaccional                                 │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ DbContext:                                               │   │
│  │ • FileExplorerDbContext (EF Core)                       │   │
│  │ • Mapeo de entidades                                    │   │
│  │ • Migraciones                                           │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             ▲
                             │ Entidades
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Entidades:                                               │   │
│  │ • User                (Usuarios del sistema)             │   │
│  │ • RefreshToken        (Tokens de refresco)              │   │
│  │ • FileSystemItem      (Base abstracta)                  │   │
│  │   ├─ FileItem         (Archivos)                        │   │
│  │   └─ DirectoryItem    (Directorios)                     │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Value Objects:                                           │   │
│  │ • Email, FirstName, LastName, Password                  │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Monads (Programación Funcional):                         │   │
│  │ • Result<T>           (Manejo de errores)               │   │
│  │ • Maybe<T>            (Valores opcionales)              │   │
│  │ • Errors              (Tipos de errores)                │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔐 Flujo de Seguridad

```
┌──────────────────────────────────────────────────────────────────┐
│                    AUTENTICACIÓN (Login)                         │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  1. Cliente envía: { email, password }                          │
│                                                                  │
│  2. AuthController.Login()                                      │
│     └─> UserServices.AuthenticateUserAsync()                   │
│                                                                  │
│  3. Validaciones:                                               │
│     ├─ ¿Usuario existe?                                        │
│     ├─ ¿Cuenta bloqueada? (LockoutEnd > DateTime.UtcNow)      │
│     └─ ¿Contraseña válida? (BCrypt.Verify)                    │
│                                                                  │
│  4. Si falla contraseña:                                        │
│     ├─ FailedLoginAttempts++                                   │
│     ├─ Si >= 5: LockoutEnd = DateTime.UtcNow + 5 minutos      │
│     └─ Guardar cambios                                         │
│                                                                  │
│  5. Si éxito:                                                   │
│     ├─ FailedLoginAttempts = 0                                 │
│     ├─ LockoutEnd = null                                       │
│     ├─ LastLoginAt = DateTime.UtcNow                           │
│     ├─ Generar JWT token (24h expiración)                      │
│     └─ Retornar LoginResponse con token                        │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│                    JWT TOKEN GENERATION                          │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Claims incluidos:                                              │
│  ├─ NameIdentifier: userId                                     │
│  ├─ Email: user.Email                                          │
│  ├─ Role: \"User\"                                              │
│  ├─ Jti: Guid.NewGuid() (ID único del token)                  │
│  └─ Iat: Unix timestamp (fecha de emisión)                     │
│                                                                  │
│  Configuración:                                                 │
│  ├─ Issuer: \"FileExplorer\"                                    │
│  ├─ Audience: \"FileExplorerUsers\"                             │
│  ├─ Expires: DateTime.UtcNow.AddHours(24)                      │
│  └─ SigningCredentials: HMAC-SHA256                            │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│                    AUTORIZACIÓN (Requests)                       │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  1. Cliente envía: Authorization: Bearer <token>               │
│                                                                  │
│  2. Middleware JWT valida:                                      │
│     ├─ Firma del token (HMAC-SHA256)                           │
│     ├─ Issuer correcto                                         │
│     ├─ Audience correcto                                       │
│     ├─ Token no expirado                                       │
│     └─ ClockSkew = 0 (sin tolerancia)                          │
│                                                                  │
│  3. Si válido:                                                  │
│     └─ Extraer claims y autorizar request                      │
│                                                                  │
│  4. Si inválido:                                                │
│     └─ Retornar 401 Unauthorized                               │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

---

## 📊 Modelo de Datos (ERD Simplificado)

```
┌─────────────────────────┐
│         User            │
├─────────────────────────┤
│ Id (PK)                 │
│ FirstName               │
│ LastName                │
│ Email (UNIQUE)          │
│ Phone                   │
│ PasswordHash            │
│ FailedLoginAttempts     │
│ LockoutEnd              │
│ StorageQuotaBytes       │
│ PasswordResetToken      │
│ PasswordResetTokenExpiry│
│ CreatedAt               │
│ UpdatedAt               │
│ IsActive                │
│ LastLoginAt             │
└────────────┬────────────┘
             │ 1:N
             │
             ▼
┌─────────────────────────┐
│    RefreshToken         │
├─────────────────────────┤
│ Id (PK)                 │
│ Token (UNIQUE)          │
│ UserId (FK)             │
│ Created                 │
│ Expire                  │
└─────────────────────────┘

┌──────────────────────────────────┐
│     FileSystemItem (Abstract)    │
├──────────────────────────────────┤
│ Name                             │
│ Path                             │
│ Size                             │
│ CreatedAt                        │
│ ModifiedAt                       │
│ IsDirectory                      │
│ ItemType (File|Directory)        │
└──────────────────────────────────┘
         ▲                ▲
         │                │
    ┌────┴────┐      ┌────┴──────────┐
    │          │      │               │
┌───┴──────┐  │  ┌────┴──────────┐   │
│ FileItem │  │  │ DirectoryItem │   │
├──────────┤  │  ├───────────────┤   │
│ (Archivo)│  │  │ (Carpeta)     │   │
└──────────┘  │  │ Files[]       │   │
              │  │ SubFolders[]  │   │
              │  └───────────────┘   │
              │         │            │
              └─────────┴────────────┘
                   1:N (Jerarquía)
```

---

## 🔄 Ciclo de Vida de una Solicitud

```
1. REQUEST INGRESA
   │
   ├─> RequestIdMiddleware
   │   └─> Asigna ID único para trazabilidad
   │
   ├─> JWT Authentication Middleware
   │   └─> Valida token Bearer
   │
   ├─> CORS Middleware
   │   └─> Valida origen
   │
   ▼
2. CONTROLLER RECIBE
   │
   ├─> AuthController.Login()
   │   └─> Valida argumentos nulos
   │
   ▼
3. APPLICATION SERVICE PROCESA
   │
   ├─> UserServices.AuthenticateUserAsync()
   │   ├─> Busca usuario (Repository)
   │   ├─> Valida contraseña
   │   ├─> Genera JWT
   │   └─> Actualiza BD (UnitOfWork)
   │
   ▼
4. INFRASTRUCTURE PERSISTE
   │
   ├─> Repository<User>.FindAsync()
   ├─> UnitOfWork.SaveChangesAsync()
   │   └─> DbContext.SaveChangesAsync()
   │
   ▼
5. DATABASE EJECUTA
   │
   ├─> SELECT * FROM Users WHERE Email = @email
   ├─> UPDATE Users SET ...
   │
   ▼
6. RESPONSE RETORNA
   │
   ├─> ApiResult<LoginResponse>
   │   ├─> Data: { email, firstName, lastName, phone, token }
   │   └─> Metadata: { statusCode: 200, message: "Login successful" }
   │
   ▼
7. CLIENTE RECIBE
   │
   └─> JSON con token JWT para futuras solicitudes
```

---

## 🛠️ Inyección de Dependencias

```
Program.cs
│
├─> AddApplicationServices()
│   ├─> IFileServices → FileServices
│   ├─> IFolderServices → FolderServices
│   ├─> IUserServices → UserServices
│   ├─> IJwtTokenService → JwtTokenService
│   ├─> IValidator<CreateUserRequest> → CreateUserRequestValidator
│   └─> Mapster Configuration
│
├─> AddInfrastructureServices()
│   ├─> DbContext → FileExplorerDbContext (SQLite)
│   ├─> IUnitOfWork → UnitOfWork
│   ├─> IUnitOfWorkAsync → UnitOfWork
│   ├─> IFileRepository → FileRepository
│   ├─> IFolderRepository → FolderRepository
│   └─> IRepositoryAsync<T> → Repository<T>
│
├─> AddAuthentication("Bearer")
│   └─> AddJwtBearer() con TokenValidationParameters
│
└─> AddCors(), AddSwaggerGen(), AddSerilog()
```

---

## 📝 Ejemplo: Flujo Completo de Registro

```
1. Cliente POST /api/v1/auth/register
   {
     "firstName": "Juan",
     "lastName": "Pérez",
     "email": "juan@example.com",
     "phone": "+34 123456789",
     "password": "SecurePass123!"
   }

2. AuthController.Register()
   └─> UserServices.RegisterUserAsync(request)

3. UserServices valida:
   ├─> FluentValidation (CreateUserRequestValidator)
   ├─> ¿Email ya existe?
   └─> ¿Contraseña cumple requisitos?

4. Si válido:
   ├─> Crear User entity
   ├─> PasswordHash = BCrypt.HashPassword(password)
   ├─> CreatedAt = DateTime.UtcNow
   ├─> IsActive = false
   ├─> Repository.Insert(user)
   └─> UnitOfWork.SaveChangesAsync()

5. Generar JWT:
   ├─> JwtTokenService.GenerateToken(userId, email, "User")
   └─> Claims: NameIdentifier, Email, Role, Jti, Iat

6. Retornar LoginResponse:
   {
     "data": {
       "email": "juan@example.com",
       "firstName": "Juan",
       "lastName": "Pérez",
       "phone": "+34 123456789",
       "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
     },
     "metadata": {
       "statusCode": 201,
       "message": "User created successfully",
       "errors": null
     }
   }

7. Cliente almacena token en localStorage
   └─> Futuras solicitudes: Authorization: Bearer <token>
```

