# File-Explorer Backend - Referencia Rápida de APIs

## 🔐 Autenticación (AuthController)

### Login
```
POST /api/v1/auth/login
Content-Type: application/json

Request:
{
  "email": "user@example.com",
  "password": "password123"
}

Response (200):
{
  "data": {
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+1234567890",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "metadata": {
    "statusCode": 200,
    "message": "Login successful",
    "errors": null
  }
}

Errores posibles:
- 401: Invalid credentials
- 403: User is locked. Try again later.
```

### Registro
```
POST /api/v1/auth/register
Content-Type: application/json

Request:
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "password": "SecurePass123!"
}

Response (201):
{
  "data": {
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+1234567890",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "metadata": {
    "statusCode": 201,
    "message": "User created successfully",
    "errors": null
  }
}

Errores posibles:
- 400: Validation errors
- 409: User already exists
```

### Refresh Token
```
POST /api/v1/auth/refresh
Content-Type: application/json

Request:
{
  "refreshToken": "refresh_token_value"
}

Response (200):
{
  "data": {
    "token": "new_jwt_token",
    "refreshToken": "new_refresh_token"
  },
  "metadata": {
    "statusCode": 200,
    "message": "Token refreshed",
    "errors": null
  }
}

Errores posibles:
- 401: Invalid refresh token
```

### Logout
```
POST /api/v1/auth/logout
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "refreshToken": "refresh_token_value"
}

Response (204): No Content
```

### Forgot Password
```
POST /api/v1/auth/forgot-password
Content-Type: application/json

Request:
{
  "email": "user@example.com"
}

Response (200):
{
  "message": "Password reset email sent"
}
```

### Reset Password
```
POST /api/v1/auth/reset-password
Content-Type: application/json

Request:
{
  "token": "reset_token_from_email",
  "newPassword": "NewSecurePass123!"
}

Response (200):
{
  "data": true,
  "metadata": {
    "statusCode": 200,
    "message": "Password reset successfully",
    "errors": null
  }
}

Errores posibles:
- 400: Invalid or expired reset token
```

### Google Auth (Pendiente)
```
POST /api/v1/auth/google
Content-Type: application/json

Request:
{
  "idToken": "google_id_token"
}

Response (200):
{
  "data": { ... },
  "metadata": { ... }
}
```

---

## 👤 Usuarios (UserController)

### Obtener Usuario por ID
```
GET /api/v1/user/{id}
Authorization: Bearer <token>

Response (200):
{
  "data": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phone": "+1234567890",
    "fullName": "John Doe",
    "storageQuotaBytes": 5368709120,
    "createdAt": "2025-02-27T10:30:00Z",
    "updatedAt": "2025-02-27T10:30:00Z",
    "isActive": true,
    "lastLoginAt": "2025-02-27T15:45:00Z"
  },
  "metadata": {
    "statusCode": 200,
    "message": "User found",
    "errors": null
  }
}

Errores posibles:
- 404: User not found
- 401: Unauthorized
```

### Actualizar Perfil de Usuario
```
PUT /api/v1/auth/editUser
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "+1987654321",
  "password": "NewPassword123!" // Opcional
}

Response (200):
{
  "data": true,
  "metadata": {
    "statusCode": 200,
    "message": "User updated successfully",
    "errors": null
  }
}

Errores posibles:
- 400: Validation errors
- 404: User not found
- 409: Email already in use
```

### Obtener Todos los Usuarios (Admin)
```
GET /api/v1/user
Authorization: Bearer <token>

Response (200):
{
  "data": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      ...
    },
    {
      "id": 2,
      "firstName": "Jane",
      "lastName": "Smith",
      "email": "jane@example.com",
      ...
    }
  ],
  "metadata": {
    "statusCode": 200,
    "message": "Users retrieved successfully",
    "errors": null
  }
}
```

---

## 📁 Archivos (FileController)

### Obtener Archivo por ID
```
GET /api/v1/file/{id}
Authorization: Bearer <token>

Response (200):
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "document.pdf",
    "path": "/uploads/2025/02/document.pdf",
    "size": 1024000,
    "createdAt": "2025-02-27T10:30:00Z",
    "modifiedAt": "2025-02-27T10:30:00Z",
    "itemType": "File"
  },
  "metadata": {
    "statusCode": 200,
    "message": "File retrieved",
    "errors": null
  }
}

Errores posibles:
- 404: File not found
- 401: Unauthorized
```

### Subir Archivo
```
POST /api/v1/file/upload
Content-Type: multipart/form-data
Authorization: Bearer <token>

Form Data:
- file: <binary file>
- folderId: 550e8400-e29b-41d4-a716-446655440000 (opcional)

Response (200):
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "document.pdf",
    "path": "/uploads/2025/02/document.pdf",
    "size": 1024000,
    "createdAt": "2025-02-27T10:30:00Z",
    "modifiedAt": "2025-02-27T10:30:00Z"
  },
  "metadata": {
    "statusCode": 200,
    "message": "File uploaded successfully",
    "errors": null
  }
}

Errores posibles:
- 400: No file provided
- 413: File too large
- 401: Unauthorized
```

### Descargar Archivo
```
GET /api/v1/file/{id}/download
Authorization: Bearer <token>

Response (200):
Content-Type: application/octet-stream
Content-Disposition: attachment; filename="document.pdf"

<binary file content>

Errores posibles:
- 404: File not found
- 401: Unauthorized
```

### Eliminar Archivo
```
DELETE /api/v1/file/{id}
Authorization: Bearer <token>

Response (204): No Content

Errores posibles:
- 404: File not found
- 401: Unauthorized
```

### Compartir Archivo
```
POST /api/v1/file/{id}/share
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "expirationDays": 7,
  "allowDownload": true,
  "password": "sharepass123" // Opcional
}

Response (200):
{
  "data": {
    "shareLink": "https://fileexplorer.com/share/abc123def456",
    "expiresAt": "2025-03-06T10:30:00Z",
    "allowDownload": true
  },
  "metadata": {
    "statusCode": 200,
    "message": "Share link created",
    "errors": null
  }
}

Errores posibles:
- 404: File not found
- 401: Unauthorized
```

---

## 📂 Carpetas (FolderController)

### Crear Carpeta
```
POST /api/v1/folder
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "name": "My Folder",
  "parentFolderId": "550e8400-e29b-41d4-a716-446655440000" // Opcional
}

Response (201):
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "name": "My Folder",
    "path": "/uploads/2025/02/My Folder",
    "size": 0,
    "createdAt": "2025-02-27T10:30:00Z",
    "modifiedAt": "2025-02-27T10:30:00Z",
    "itemType": "Directory"
  },
  "metadata": {
    "statusCode": 201,
    "message": "Folder created successfully",
    "errors": null
  }
}

Errores posibles:
- 400: Invalid folder name
- 409: Folder already exists
- 401: Unauthorized
```

### Obtener Contenido de Carpeta
```
GET /api/v1/folder/{id}
Authorization: Bearer <token>

Response (200):
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "name": "My Folder",
    "path": "/uploads/2025/02/My Folder",
    "size": 2048000,
    "createdAt": "2025-02-27T10:30:00Z",
    "modifiedAt": "2025-02-27T10:30:00Z",
    "itemType": "Directory",
    "files": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440001",
        "name": "document.pdf",
        "size": 1024000,
        "itemType": "File"
      }
    ],
    "subFolders": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440003",
        "name": "Subfolder",
        "size": 1024000,
        "itemType": "Directory"
      }
    ]
  },
  "metadata": {
    "statusCode": 200,
    "message": "Folder retrieved",
    "errors": null
  }
}

Errores posibles:
- 404: Folder not found
- 401: Unauthorized
```

### Renombrar Carpeta
```
PUT /api/v1/folder/{id}
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "name": "New Folder Name"
}

Response (200):
{
  "data": true,
  "metadata": {
    "statusCode": 200,
    "message": "Folder renamed successfully",
    "errors": null
  }
}

Errores posibles:
- 404: Folder not found
- 409: Folder name already exists
- 401: Unauthorized
```

### Eliminar Carpeta
```
DELETE /api/v1/folder/{id}
Authorization: Bearer <token>

Response (204): No Content

Errores posibles:
- 404: Folder not found
- 409: Folder not empty
- 401: Unauthorized
```

---

## 🔍 Búsqueda (SearchController)

### Buscar Archivos
```
GET /api/v1/search?query=document&type=file&size=1024000
Authorization: Bearer <token>

Query Parameters:
- query: Término de búsqueda (requerido)
- type: file|folder (opcional)
- size: Tamaño máximo en bytes (opcional)
- createdAfter: Fecha ISO (opcional)
- createdBefore: Fecha ISO (opcional)

Response (200):
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "document.pdf",
      "path": "/uploads/2025/02/document.pdf",
      "size": 1024000,
      "itemType": "File"
    }
  ],
  "metadata": {
    "statusCode": 200,
    "message": "Search completed",
    "errors": null
  }
}

Errores posibles:
- 400: Invalid search parameters
- 401: Unauthorized
```

---

## 🔐 Permisos (PermissionController)

### Asignar Permiso
```
POST /api/v1/permission
Content-Type: application/json
Authorization: Bearer <token>

Request:
{
  "userId": 2,
  "resourceId": "550e8400-e29b-41d4-a716-446655440001",
  "resourceType": "file",
  "permissionType": "read" // read|write|delete|admin
}

Response (201):
{
  "data": {
    "id": 1,
    "userId": 2,
    "resourceId": "550e8400-e29b-41d4-a716-446655440001",
    "permissionType": "read",
    "grantedAt": "2025-02-27T10:30:00Z"
  },
  "metadata": {
    "statusCode": 201,
    "message": "Permission granted",
    "errors": null
  }
}

Errores posibles:
- 404: User or resource not found
- 409: Permission already exists
- 401: Unauthorized
```

### Revocar Permiso
```
DELETE /api/v1/permission/{id}
Authorization: Bearer <token>

Response (204): No Content

Errores posibles:
- 404: Permission not found
- 401: Unauthorized
```

### Obtener Permisos de Usuario
```
GET /api/v1/permission/user/{userId}
Authorization: Bearer <token>

Response (200):
{
  "data": [
    {
      "id": 1,
      "userId": 2,
      "resourceId": "550e8400-e29b-41d4-a716-446655440001",
      "permissionType": "read",
      "grantedAt": "2025-02-27T10:30:00Z"
    }
  ],
  "metadata": {
    "statusCode": 200,
    "message": "Permissions retrieved",
    "errors": null
  }
}

Errores posibles:
- 404: User not found
- 401: Unauthorized
```

---

## 🏥 Health Check

### Estado de la API
```
GET /api/v1/health

Response (200):
{
  "status": "healthy",
  "timestamp": "2025-02-27T10:30:00Z",
  "version": "1.0.0",
  "database": "connected"
}
```

---

## 📋 Códigos de Estado HTTP

| Código | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Solicitud exitosa |
| 201 | Created | Recurso creado exitosamente |
| 204 | No Content | Solicitud exitosa sin contenido |
| 400 | Bad Request | Datos inválidos o incompletos |
| 401 | Unauthorized | Token inválido o expirado |
| 403 | Forbidden | Acceso denegado (cuenta bloqueada) |
| 404 | Not Found | Recurso no encontrado |
| 409 | Conflict | Conflicto (duplicado, etc.) |
| 413 | Payload Too Large | Archivo demasiado grande |
| 500 | Internal Server Error | Error del servidor |

---

## 🔑 Headers Requeridos

### Autenticación
```
Authorization: Bearer <jwt_token>
```

### Content-Type
```
Content-Type: application/json
```

### Para multipart/form-data
```
Content-Type: multipart/form-data
```

---

## 📊 Estructura de Errores

### Error simple
```json
{
  "data": null,
  "metadata": {
    "statusCode": 400,
    "message": "Invalid email format",
    "errors": null
  }
}
```

### Errores de validación
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

## 🧪 Ejemplo: Flujo Completo

```bash
# 1. Registrar usuario
curl -X POST https://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phone": "+1234567890",
    "password": "SecurePass123!"
  }'

# Respuesta: { "data": { "token": "eyJ..." }, ... }

# 2. Usar token para crear carpeta
curl -X POST https://localhost:5001/api/v1/folder \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{
    "name": "My Documents"
  }'

# Respuesta: { "data": { "id": "550e8400..." }, ... }

# 3. Subir archivo a la carpeta
curl -X POST https://localhost:5001/api/v1/file/upload \
  -H "Authorization: Bearer eyJ..." \
  -F "file=@document.pdf" \
  -F "folderId=550e8400..."

# Respuesta: { "data": { "id": "550e8400..." }, ... }

# 4. Compartir archivo
curl -X POST https://localhost:5001/api/v1/file/550e8400.../share \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{
    "expirationDays": 7,
    "allowDownload": true
  }'

# Respuesta: { "data": { "shareLink": "https://..." }, ... }
```

