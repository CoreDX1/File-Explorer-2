# File-Explorer Backend - Referencia Rápida (Tablas)

## 🏗️ Componentes por Capa

### Domain Layer

| Componente | Ubicación | Responsabilidad |
|-----------|-----------|-----------------|
| `User` | `Domain/Entities/User.cs` | Entidad de usuario con autenticación |
| `RefreshToken` | `Domain/Entities/RefreshToken.cs` | Token de refresco JWT |
| `FileSystemItem` | `Domain/Entities/FileSystemItem.cs` | Base abstracta para archivos/carpetas |
| `FileItem` | `Domain/Entities/FileItem.cs` | Representa un archivo |
| `DirectoryItem` | `Domain/Entities/DirectoryItem.cs` | Representa una carpeta |
| `Result<T>` | `Domain/Monads/Result/Result.cs` | Manejo de errores funcional |
| `Maybe<T>` | `Domain/Monads/Maybe.cs` | Valores opcionales seguros |
| `Email` | `Domain/ValueObjects/Email.cs` | Value Object para email |
| `Password` | `Domain/ValueObjects/Password.cs` | Value Object para contraseña |

### Application Layer

| Componente | Ubicación | Responsabilidad |
|-----------|-----------|-----------------|
| `UserServices` | `Application/Services/UserServices.cs` | Lógica de autenticación y usuarios |
| `FileServices` | `Application/Services/FileServices.cs` | Operaciones con archivos |
| `FolderServices` | `Application/Services/FolderServices.cs` | Operaciones con carpetas |
| `JwtTokenService` | `Application/Services/JwtTokenService.cs` | Generación y validación de JWT |
| `IUserServices` | `Application/Interfaces/IUserServices.cs` | Contrato de servicios de usuario |
| `IFileServices` | `Application/Interfaces/IFileServices.cs` | Contrato de servicios de archivo |
| `IFolderServices` | `Application/Interfaces/IFolderServices.cs` | Contrato de servicios de carpeta |
| `CreateUserRequest` | `Application/DTOs/Request/CreateUserRequest.cs` | DTO para crear usuario |
| `LoginRequest` | `Application/DTOs/Request/LoginRequest.cs` | DTO para login |
| `LoginResponse` | `Application/DTOs/Response/LoginResponse.cs` | DTO de respuesta de login |
| `ApiResult<T>` | `Application/DTOs/Response/Response.cs` | Envoltorio de respuesta API |

### Infrastructure Layer

| Componente | Ubicación | Responsabilidad |
|-----------|-----------|-----------------|
| `FileExplorerDbContext` | `Infrastructure/Data/FileExplorerDbContext.cs` | DbContext de EF Core |
| `Repository<T>` | `Infrastructure/Repositories/Repository.cs` | Repositorio genérico |
| `FileRepository` | `Infrastructure/Repositories/FileRepository.cs` | Repositorio específico de archivos |
| `FolderRepository` | `Infrastructure/Repositories/FolderRepository.cs` | Repositorio específico de carpetas |
| `RefreshTokenRepository` | `Infrastructure/Repositories/RefreshTokenRepository.cs` | Repositorio de tokens |
| `UnitOfWork` | `Infrastructure/UnitOfWork.cs` | Patrón Unit of Work |
| `IUnitOfWork` | `Infrastructure/Interfaces/IUnitOfWork.cs` | Contrato de Unit of Work |
| `IRepository<T>` | `Infrastructure/Interfaces/IRepository.cs` | Contrato de repositorio |

### Web Layer

| Componente | Ubicación | Responsabilidad |
|-----------|-----------|-----------------|
| `AuthController` | `Web/Controllers/AuthController.cs` | Endpoints de autenticación |
| `FileController` | `Web/Controllers/FileController.cs` | Endpoints de archivos |
| `FolderController` | `Web/Controllers/FolderController.cs` | Endpoints de carpetas |
| `UserController` | `Web/Controllers/UserController.cs` | Endpoints de usuarios |
| `SearchController` | `Web/Controllers/SearchController.cs` | Endpoints de búsqueda |
| `PermissionController` | `Web/Controllers/PermissionController.cs` | Endpoints de permisos |
| `RequestIdMiddleware` | `Web/Middleware/RequestIdMiddleware.cs` | Middleware de trazabilidad |
| `Program.cs` | `Web/Program.cs` | Configuración principal |

---

## 🔐 Métodos de Autenticación

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `Login` | `POST /auth/login` | email, password | LoginResponse + JWT | 200, 401, 403 |
| `Register` | `POST /auth/register` | firstName, lastName, email, phone, password | LoginResponse + JWT | 201, 400, 409 |
| `RefreshToken` | `POST /auth/refresh` | refreshToken | Nuevo JWT | 200, 401 |
| `Logout` | `POST /auth/logout` | refreshToken | - | 204 |
| `ForgotPassword` | `POST /auth/forgot-password` | email | Mensaje | 200 |
| `ResetPassword` | `POST /auth/reset-password` | token, newPassword | bool | 200, 400 |

---

## 👤 Métodos de Usuario

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `GetUserById` | `GET /user/{id}` | id | UserResponse | 200, 404 |
| `GetAllUsers` | `GET /user` | - | List<UserResponse> | 200 |
| `UpdateProfile` | `PUT /auth/editUser` | id, firstName, lastName, email, phone, password | bool | 200, 400, 404, 409 |
| `FindByEmail` | (Interno) | email | Maybe<User> | - |

---

## 📁 Métodos de Archivo

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `GetFile` | `GET /file/{id}` | id | FileResponse | 200, 404 |
| `UploadFile` | `POST /file/upload` | file, folderId | FileResponse | 200, 400, 413 |
| `DownloadFile` | `GET /file/{id}/download` | id | FileStream | 200, 404 |
| `DeleteFile` | `DELETE /file/{id}` | id | - | 204, 404 |
| `ShareFile` | `POST /file/{id}/share` | id, expirationDays, allowDownload | ShareLink | 200, 404 |

---

## 📂 Métodos de Carpeta

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `CreateFolder` | `POST /folder` | name, parentFolderId | FolderResponse | 201, 400, 409 |
| `GetFolder` | `GET /folder/{id}` | id | FolderResponse | 200, 404 |
| `RenameFolder` | `PUT /folder/{id}` | id, name | bool | 200, 404, 409 |
| `DeleteFolder` | `DELETE /folder/{id}` | id | - | 204, 404, 409 |

---

## 🔍 Métodos de Búsqueda

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `Search` | `GET /search` | query, type, size, createdAfter, createdBefore | List<SearchResult> | 200, 400 |

---

## 🔐 Métodos de Permiso

| Método | Endpoint | Parámetros | Retorna | Códigos |
|--------|----------|-----------|---------|---------|
| `GrantPermission` | `POST /permission` | userId, resourceId, resourceType, permissionType | PermissionResponse | 201, 404, 409 |
| `RevokePermission` | `DELETE /permission/{id}` | id | - | 204, 404 |
| `GetUserPermissions` | `GET /permission/user/{userId}` | userId | List<PermissionResponse> | 200, 404 |

---

## 📊 Entidades y Propiedades

### User
| Propiedad | Tipo | Requerido | Descripción |
|-----------|------|----------|-------------|
| Id | int | ✅ | Identificador único |
| FirstName | string | ✅ | Nombre |
| LastName | string | ✅ | Apellido |
| Email | string | ✅ | Email único |
| Phone | string | ❌ | Teléfono |
| PasswordHash | string | ✅ | Contraseña hasheada |
| FailedLoginAttempts | int | ✅ | Intentos fallidos |
| LockoutEnd | DateTime? | ❌ | Fecha de desbloqueo |
| StorageQuotaBytes | long | ✅ | Cuota de almacenamiento |
| PasswordResetToken | string? | ❌ | Token de reset |
| PasswordResetTokenExpiry | DateTime? | ❌ | Expiración del token |
| CreatedAt | DateTime | ✅ | Fecha de creación |
| UpdatedAt | DateTime | ✅ | Fecha de actualización |
| IsActive | bool | ✅ | Estado activo |
| LastLoginAt | DateTime | ❌ | Último login |

### RefreshToken
| Propiedad | Tipo | Requerido | Descripción |
|-----------|------|----------|-------------|
| Id | int | ✅ | Identificador único |
| Token | string | ✅ | Valor del token |
| UserId | int | ✅ | ID del usuario |
| Created | DateTime | ✅ | Fecha de creación |
| Expire | DateTime | ✅ | Fecha de expiración |

### FileItem
| Propiedad | Tipo | Requerido | Descripción |
|-----------|------|----------|-------------|
| Name | string | ✅ | Nombre del archivo |
| Path | string | ✅ | Ruta del archivo |
| Size | long | ✅ | Tamaño en bytes |
| CreatedAt | DateTime | ✅ | Fecha de creación |
| ModifiedAt | DateTime | ✅ | Fecha de modificación |
| ItemType | FileSystemItemType | ✅ | Tipo (File) |

### DirectoryItem
| Propiedad | Tipo | Requerido | Descripción |
|-----------|------|----------|-------------|
| Name | string | ✅ | Nombre de la carpeta |
| Path | string | ✅ | Ruta de la carpeta |
| Size | long | ✅ | Tamaño total |
| CreatedAt | DateTime | ✅ | Fecha de creación |
| ModifiedAt | DateTime | ✅ | Fecha de modificación |
| Files | ICollection<FileItem> | ✅ | Archivos contenidos |
| SubFolders | ICollection<DirectoryItem> | ✅ | Subcarpetas |
| ItemType | FileSystemItemType | ✅ | Tipo (Directory) |

---

## 🔄 Flujos de Datos

### Flujo de Login
```
Request → AuthController.Login()
        → UserServices.AuthenticateUserAsync()
        → Repository<User>.FindAsync()
        → BCrypt.Verify()
        → JwtTokenService.GenerateToken()
        → UnitOfWork.SaveChangesAsync()
        → Response (LoginResponse)
```

### Flujo de Upload
```
Request → FileController.UploadFile()
        → FileServices.UploadFileAsync()
        → Validar usuario y cuota
        → Guardar en disco
        → Repository<FileItem>.Insert()
        → UnitOfWork.SaveChangesAsync()
        → Response (FileResponse)
```

### Flujo de Búsqueda
```
Request → SearchController.Search()
        → SearchServices.SearchAsync()
        → Repository<FileItem>.Queryable()
        → Aplicar filtros
        → ToListAsync()
        → Response (List<SearchResult>)
```

---

## 🛡️ Validaciones

| Entidad | Campo | Regla | Mensaje |
|---------|-------|-------|---------|
| User | Email | Required, EmailAddress, MaxLength(100) | Email is required, Invalid format |
| User | Password | Required, MinLength(8), Regex | Password must be at least 8 characters |
| User | FirstName | Required, MaxLength(50) | First name is required |
| User | LastName | Required, MaxLength(50) | Last name is required |
| FileItem | Name | Required, MaxLength(255) | File name is required |
| DirectoryItem | Name | Required, MaxLength(255) | Folder name is required |

---

## 📈 Códigos de Estado HTTP

| Código | Nombre | Uso |
|--------|--------|-----|
| 200 | OK | Solicitud exitosa |
| 201 | Created | Recurso creado |
| 204 | No Content | Solicitud exitosa sin contenido |
| 400 | Bad Request | Datos inválidos |
| 401 | Unauthorized | Token inválido/expirado |
| 403 | Forbidden | Acceso denegado |
| 404 | Not Found | Recurso no encontrado |
| 409 | Conflict | Conflicto (duplicado) |
| 413 | Payload Too Large | Archivo demasiado grande |
| 500 | Internal Server Error | Error del servidor |

---

## 🔑 Headers HTTP

| Header | Valor | Requerido | Descripción |
|--------|-------|----------|-------------|
| Authorization | Bearer <token> | ✅ (en endpoints protegidos) | Token JWT |
| Content-Type | application/json | ✅ (en POST/PUT) | Tipo de contenido |
| Content-Type | multipart/form-data | ✅ (en upload) | Para archivos |
| Accept | application/json | ❌ | Tipo aceptado |

---

## 🔐 Claims JWT

| Claim | Tipo | Descripción |
|-------|------|-------------|
| NameIdentifier | string | ID del usuario |
| Email | string | Email del usuario |
| Role | string | Rol del usuario |
| Jti | string | ID único del token |
| Iat | long | Timestamp de emisión |
| Exp | long | Timestamp de expiración |

---

## 📝 Configuración (appsettings.json)

| Sección | Clave | Tipo | Descripción |
|---------|-------|------|-------------|
| ConnectionStrings | DefaultConnection | string | Conexión a BD |
| JwtSettings | SecretKey | string | Clave secreta (min 32 chars) |
| JwtSettings | Issuer | string | Emisor del token |
| JwtSettings | Audience | string | Audiencia del token |
| JwtSettings | ExpirationHours | int | Horas de expiración |
| FileStorage | ContainerPath | string | Ruta de almacenamiento |

---

## 🧪 Ejemplos de Requests

### Login
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

### Registro
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "password": "SecurePass123!"
}
```

### Crear Carpeta
```json
{
  "name": "My Documents",
  "parentFolderId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### Compartir Archivo
```json
{
  "expirationDays": 7,
  "allowDownload": true,
  "password": "sharepass123"
}
```

### Actualizar Perfil
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "+1987654321",
  "password": "NewPassword123!"
}
```

---

## 📊 Estadísticas del Proyecto

| Métrica | Valor |
|---------|-------|
| Capas de arquitectura | 4 |
| Controladores | 6 |
| Servicios | 4 |
| Repositorios | 4 |
| Entidades | 5 |
| DTOs | 10+ |
| Endpoints | 20+ |
| Migraciones | 7 |
| Validadores | 1+ |

---

## 🚀 Performance

| Operación | Tiempo Esperado | Notas |
|-----------|-----------------|-------|
| Login | < 100ms | Incluye validación de contraseña |
| Upload archivo | Depende del tamaño | Streaming recomendado |
| Búsqueda | < 500ms | Con índices en BD |
| Crear carpeta | < 50ms | Operación simple |
| Listar archivos | < 200ms | Depende de cantidad |

---

## 🔗 Relaciones de Entidades

```
User (1) ──────────── (N) RefreshToken
User (1) ──────────── (N) FileItem
User (1) ──────────── (N) DirectoryItem
DirectoryItem (1) ──────────── (N) FileItem
DirectoryItem (1) ──────────── (N) DirectoryItem (SubFolders)
```

---

## 📚 Archivos Clave

| Archivo | Líneas | Responsabilidad |
|---------|--------|-----------------|
| Program.cs | ~100 | Configuración principal |
| UserServices.cs | ~400 | Lógica de autenticación |
| JwtTokenService.cs | ~80 | Generación de JWT |
| FileExplorerDbContext.cs | ~50 | Mapeo de entidades |
| UnitOfWork.cs | ~120 | Transaccionalidad |
| AuthController.cs | ~100 | Endpoints de auth |
| Repository.cs | ~150 | Operaciones genéricas |

---

## 🎯 Próximos Pasos

1. **Leer documentación:** Comenzar con [BACKEND_CONTEXT.md](./BACKEND_CONTEXT.md)
2. **Entender arquitectura:** Revisar [ARCHITECTURE_DIAGRAMS.md](./ARCHITECTURE_DIAGRAMS.md)
3. **Aprender patrones:** Estudiar [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)
4. **Consultar APIs:** Usar [API_REFERENCE.md](./API_REFERENCE.md)
5. **Desarrollar:** Crear nuevas funcionalidades siguiendo los patrones

