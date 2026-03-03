# 📋 Backend Implementation TODO List

**Ordenado de lo más fácil a lo más difícil**

*Última actualización: Marzo 3, 2026*

---

## 🟢 Nivel 1: Tareas Simples (Configuración y Mejoras Menores)

### 1.1 Configuración de Rate Limiting
- [ ] Instalar paquete `AspNetCore.RateLimit`
- [ ] Configurar middleware en `Program.cs`
- [ ] Definir políticas por endpoint (login: 5/min, upload: 10/min)
- [ ] Agregar respuestas HTTP 429 personalizadas
- **Archivos:** `src/Web/Program.cs`, `src/Web/appsettings.json`
- **Dificultad:** 2/10
- **Tiempo estimado:** 1-2 horas

### 1.2 CSRF Protection
- [ ] Habilitar antiforgery tokens en configuración ASP.NET Core
- [ ] Agregar validación en endpoints sensibles (POST, PUT, DELETE)
- [ ] Documentar uso en API_REFERENCE.md
- **Archivos:** `src/Web/Program.cs`, controladores
- **Dificultad:** 3/10
- **Tiempo estimado:** 2-3 horas

### 1.3 Health Check Mejorado
- [ ] Agregar checks de disco disponible
- [ ] Verificar estado de conexión a BD
- [ ] Incluir métricas básicas (usuarios activos, archivos totales)
- [ ] Agregar endpoint `/health/ready` y `/health/live`
- **Archivos:** `src/Web/Controllers/HealthController.cs` (nuevo)
- **Dificultad:** 3/10
- **Tiempo estimado:** 2-3 horas

### 1.4 Logging de Auditoría Básico
- [ ] Crear entidad `AuditLog` en Domain
- [ ] Loggear operaciones críticas (login, delete, share)
- [ ] Agregar endpoint para consultar logs (admin)
- **Archivos:** `Domain/Entities/AuditLog.cs`, middleware, `src/Web/Controllers/AuditController.cs`
- **Dificultad:** 4/10
- **Tiempo estimado:** 3-4 horas

---

## 🟡 Nivel 2: Tareas de Complejidad Media (Características Existentes)

### 2.1 Token Refresh Ininterrumpido
- [ ] Implementar endpoint `/api/v1/auth/refresh` completo
- [ ] Validar refresh token contra BD
- [ ] Generar nuevo par JWT + refresh token
- [ ] Revocar token anterior
- [ ] Manejar rotación de tokens
- **Archivos:** `AuthController.cs`, `UserServices.cs`, `RefreshTokenRepository.cs`
- **Dificultad:** 5/10
- **Tiempo estimado:** 4-6 horas

### 2.2 Búsqueda Avanzada con Filtros
- [ ] Agregar parámetros: `orderBy`, `sortOrder`, `page`, `pageSize`
- [ ] Filtros por: tipo MIME, tags, usuario propietario
- [ ] Implementar búsqueda por contenido (nombre, path)
- [ ] Agregar paginación en respuestas
- **Archivos:** `SearchController.cs`, `FileRepository.cs`, `FileServices.cs`
- **Dificultad:** 5/10
- **Tiempo estimado:** 5-7 horas

### 2.3 Enlaces Públicos con Expiración
- [ ] Crear entidad `ShareLink` con token único
- [ ] Generar links con expiración configurable
- [ ] Endpoint público `/s/{token}` para acceder
- [ ] Validar expiración y permisos
- [ ] Opcional: protección con contraseña
- **Archivos:** `Domain/Entities/ShareLink.cs`, `ShareController.cs` (nuevo)
- **Dificultad:** 6/10
- **Tiempo estimado:** 6-8 horas

### 2.4 Google OAuth (SSO)
- [ ] Registrar aplicación en Google Cloud Console
- [ ] Instalar `Microsoft.AspNetCore.Authentication.Google`
- [ ] Configurar OAuth en `Program.cs`
- [ ] Agregar endpoint `/api/v1/auth/google`
- [ ] Crear/actualizar usuario desde claims de Google
- [ ] Unificar con sistema JWT existente
- **Archivos:** `AuthController.cs`, `Program.cs`, `appsettings.json`
- **Dificultad:** 6/10
- **Tiempo estimado:** 6-8 horas

### 2.5 RBAC Granular (Roles y Permisos)
- [ ] Crear entidad `Role` (Admin, User, Guest)
- [ ] Crear entidad `Permission` (Create, Read, Update, Delete)
- [ ] Asignar roles a usuarios
- [ ] Validar permisos en cada endpoint
- [ ] Agregar endpoints para gestión de roles (admin)
- **Archivos:** `Domain/Entities/Role.cs`, `Permission.cs`, middleware de autorización
- **Dificultad:** 7/10
- **Tiempo estimado:** 8-10 horas

---

## 🟠 Nivel 3: Tareas Complejas (Integraciones y Optimización)

### 3.1 Notificaciones en Tiempo Real
- [ ] Instalar SignalR (`Microsoft.AspNetCore.SignalR`)
- [ ] Crear hub `NotificationHub`
- [ ] Notificar eventos: archivo compartido, permiso asignado
- [ ] Integrar con frontend Angular
- [ ] Manejar reconexiones y offline
- **Archivos:** `src/Web/Hubs/NotificationHub.cs`, servicios de notificación
- **Dificultad:** 7/10
- **Tiempo estimado:** 8-12 horas

### 3.2 Análisis de Almacenamiento
- [ ] Calcular uso por usuario (sumar tamaños de archivos)
- [ ] Alertas al alcanzar 80%, 90%, 100% de cuota
- [ ] Dashboard con estadísticas (archivos por tipo, evolución)
- [ ] Endpoint `/api/v1/user/{id}/storage`
- **Archivos:** `FileServices.cs`, `UserServices.cs`, nuevo controller
- **Dificultad:** 7/10
- **Tiempo estimado:** 8-10 horas

### 3.3 Políticas de Retención
- [ ] Crear entidad `RetentionPolicy` (días, acción)
- [ ] Archivos inactivos > X días → notificar
- [ ] Archivos inactivos > Y días → eliminar (configurable)
- [ ] Background job para ejecutar políticas
- [ ] Soft delete con recuperación (opcional)
- **Archivos:** `Domain/Entities/RetentionPolicy.cs`, background service
- **Dificultad:** 8/10
- **Tiempo estimado:** 10-15 horas

### 3.4 Integración con Servicios Externos
- [ ] **Opción A:** AWS S3 para almacenamiento
  - [ ] Instalar `AWSSDK.S3`
  - [ ] Crear servicio `S3StorageService`
  - [ ] Migrar archivos locales a S3
- [ ] **Opción B:** SendGrid para emails
  - [ ] Instalar `SendGrid`
  - [ ] Reemplazar envío de emails (reset password)
  - [ ] Templates HTML
- **Archivos:** `Infrastructure/Services/External/`
- **Dificultad:** 8/10
- **Tiempo estimado:** 10-15 horas

---

## 🔴 Nivel 4: Tareas Muy Complejas (Arquitectura y Escalabilidad)

### 4.1 Caché Distribuido (Redis)
- [ ] Instalar Redis (`StackExchange.Redis`)
- [ ] Cachear: datos de usuario, permisos, búsquedas frecuentes
- [ ] Invalidar caché en escrituras
- [ ] Configurar expiración por tipo de dato
- [ ] Manejar conexión fallback a BD
- **Archivos:** `Infrastructure/Cache/RedisCacheService.cs`, servicios existentes
- **Dificultad:** 8/10
- **Tiempo estimado:** 12-16 horas

### 4.2 Message Queue (RabbitMQ / Azure Service Bus)
- [ ] Instalar paquete de mensajería
- [ ] Cola para: emails, procesamiento de archivos, notificaciones
- [ ] Publicar eventos: `FileUploaded`, `UserRegistered`
- [ ] Consumidores asíncronos
- [ ] Manejo de reintentos y dead-letter queue
- **Archivos:** `Infrastructure/Messaging/`, background services
- **Dificultad:** 9/10
- **Tiempo estimado:** 15-20 horas

### 4.3 Búsqueda Avanzada (Elasticsearch)
- [ ] Instalar Elasticsearch + cliente NEST
- [ ] Indexar archivos: nombre, path, metadata, contenido (OCR)
- [ ] Búsqueda full-text con relevancia
- [ ] Filtros facetados (tipo, fecha, tamaño, tags)
- [ ] Sincronización BD ↔ Elasticsearch
- **Archivos:** `Infrastructure/Search/ElasticsearchService.cs`
- **Dificultad:** 9/10
- **Tiempo estimado:** 20-30 horas

### 4.4 Replicación de Base de Datos
- [ ] Configurar lectura/escritura separadas
- [ ] Replicación maestro-esclavo SQLite → PostgreSQL
- [ ] Routing de queries (lectura → réplica, escritura → maestro)
- [ ] Manejar consistencia eventual
- [ ] Migración de datos
- **Archivos:** `Infrastructure/Data/`, configuración de conexión
- **Dificultad:** 10/10
- **Tiempo estimado:** 30-40 horas

### 4.5 Load Balancing + Horizontal Scaling
- [ ] Containerizar con Docker
- [ ] Configurar load balancer (NGINX, Traefik)
- [ ] Sesiones stateless (JWT sin estado)
- [ ] Storage compartido (S3, NFS)
- [ ] Health checks para orquestador
- **Archivos:** `Dockerfile`, `docker-compose.yml`, configuración de infraestructura
- **Dificultad:** 10/10
- **Tiempo estimado:** 20-30 horas

---

## 📊 Resumen por Nivel

| Nivel | Cantidad | Dificultad | Tiempo Total Estimado |
|-------|----------|------------|----------------------|
| 🟢 Simple | 4 tareas | 2-4/10 | 8-12 horas |
| 🟡 Media | 5 tareas | 5-7/10 | 33-43 horas |
| 🟠 Compleja | 4 tareas | 7-8/10 | 36-50 horas |
| 🔴 Muy Compleja | 5 tareas | 8-10/10 | 97-136 horas |

**Total General:** 18 tareas | **174-241 horas** (~4-6 semanas full-time)

---

## 🎯 Recomendaciones de Implementación

### Ruta Crítica Mínima (MVP)
1. Token Refresh (2.1) - Esencial para UX
2. Enlaces Públicos (2.3) - Feature clave
3. RBAC Granular (2.5) - Seguridad básica
4. Análisis de Almacenamiento (3.2) - Control de cuota

### Prioridad Alta (Producción)
1. Rate Limiting (1.1) - Protección básica
2. Health Check Mejorado (1.3) - Monitoreo
3. Logging de Auditoría (1.4) - Compliance
4. Búsqueda Avanzada (2.2) - UX

### Prioridad Media (Crecimiento)
1. Google OAuth (2.4) - Onboarding fácil
2. Notificaciones (3.1) - Engagement
3. Políticas de Retención (3.3) - Gestión de espacio

### Prioridad Baja (Escalabilidad)
1. Caché Redis (4.1) - Performance
2. Message Queue (4.2) - Desacople
3. Elasticsearch (4.3) - Búsqueda enterprise
4. Replicación BD (4.4) - Escalabilidad
5. Load Balancing (4.5) - High availability

---

## 📝 Notas Adicionales

### Dependencias entre Tareas
- **RBAC (2.5)** depende de → Token Refresh (2.1)
- **Notificaciones (3.1)** se beneficia de → Message Queue (4.2)
- **Elasticsearch (4.3)** requiere → Búsqueda Avanzada (2.2)
- **Load Balancing (4.5)** requiere → Caché (4.1) + Storage externo (3.4)

### Riesgos Técnicos
- Migrar de SQLite a PostgreSQL puede requerir cambios en migraciones
- Implementar Redis introduce punto único de fallo (necesita cluster)
- Elasticsearch añade complejidad operacional significativa

### Criterios de Aceptación Comunes
- [ ] Tests unitarios para nueva lógica
- [ ] Tests de integración para endpoints
- [ ] Documentación actualizada en `context/`
- [ ] Logs apropiados con Serilog
- [ ] Manejo de errores con `Result<T>`
- [ ] Validación con FluentValidation

---

## 🔄 Cómo Usar Este Documento

1. **Seleccionar tarea** según prioridad y capacidad del equipo
2. **Crear rama Git** con nombre descriptivo (`feature/token-refresh`)
3. **Seguir patrones** en `DEVELOPMENT_GUIDE.md`
4. **Actualizar API_REFERENCE.md** si hay nuevos endpoints
5. **Ejecutar tests** antes de hacer commit
6. **Marcar tarea completada** en este documento

---

**Estado del Proyecto:** En desarrollo activo  
**Próximo Sprint:** Token Refresh + Enlaces Públicos  
**Versión Objetivo:** 1.1.0
