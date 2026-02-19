<h1 align="center">File-Explorer</h1>

<p align="center">
  <strong>Plataforma integral para la gestión, administración y control seguro de archivos y directorios corporativos.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Frontend-Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Frontend Angular">
  <img src="https://img.shields.io/badge/Backend-.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="Backend .NET Core">
</p>

---

## Resumen Ejecutivo

**File-Explorer** es una solución web de alto rendimiento diseñada para centralizar y optimizar la gestión de activos digitales. Construida sobre una arquitectura robusta utilizando **Angular** para la interfaz de usuario y **.NET Core** para los servicios backend, la plataforma garantiza escalabilidad, seguridad y una experiencia de usuario fluida al navegar e interactuar con sistemas de archivos complejos.

## Capacidades Principales

- **Gestión Documental Avanzada:** Navegación jerárquica, creación, renombrado y eliminación estructurada de archivos y directorios.
- **Operaciones de Transferencia:** Soporte transaccional para operaciones de copiado y reubicación de activos.
- **Motor de Búsqueda:** Indexación y localización rápida de documentos dentro del ecosistema.
- **Auditoría y Trazabilidad:** Historial de operaciones y seguimiento de modificaciones.
- **Administración Centralizada:** Panel de control para la gestión de usuarios, cuotas de almacenamiento y métricas del sistema.

---

## Módulos del Sistema

A continuación se detalla la interfaz gráfica de la plataforma, dividida por sus módulos funcionales principales.

### Panel de Control y Navegación Principal

El área de trabajo central diseñada para maximizar la productividad y ofrecer una visualización clara de la estructura de directorios.

<p align="center">
  <img src="image/Explorer.png" alt="Explorador Principal" width="80%" style="border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);">
</p>

### Identidad y Control de Acceso

Gestión de credenciales, autenticación segura y administración de perfiles de usuario.

<table align="center" width="100%">
  <tr>
    <td align="center" width="50%"><strong>Portal de Acceso</strong><br><br><img src="image/Login.png" width="350"></td>
    <td align="center" width="50%"><strong>Perfil de Usuario</strong><br><br><img src="image/Profile.png" width="350"></td>
  </tr>
</table>

### Gestión Operativa y Auditoría

Herramientas dedicadas a los administradores del sistema para el monitoreo, gestión de usuarios y generación de informes operativos.

<table align="center" width="100%">
  <tr>
    <td align="center" width="50%"><strong>Dashboard Administrativo</strong><br><br><img src="image/Admin.png" width="350"></td>
    <td align="center" width="50%"><strong>Gestor de Administradores</strong><br><br><img src="image/AdminManager.png" width="350"></td>
  </tr>
  <tr>
    <td align="center" width="50%"><strong>Reportes Analíticos</strong><br><br><img src="image/AdminReports.png" width="350"></td>
    <td align="center" width="50%"><strong>Gestión de Grupos y Permisos</strong><br><br><img src="image/GroupManage.png" width="350"></td>
  </tr>
</table>

### Gestión de Activos y Colaboración

Interfaces orientadas a la manipulación directa de datos, políticas de retención y opciones de uso compartido.

<table align="center" width="100%">
  <tr>
    <td align="center" width="50%"><strong>Estructura de Carpetas</strong><br><br><img src="image/Folder.png" width="350"></td>
    <td align="center" width="50%"><strong>Centro de Uso Compartido</strong><br><br><img src="image/FileSharing.png" width="350"></td>
  </tr>
  <tr>
    <td align="center" width="50%"><strong>Búsqueda Especializada</strong><br><br><img src="image/Search.png" width="350"></td>
    <td align="center" width="50%"><strong>Retención (Papelera)</strong><br><br><img src="image/Trash.png" width="350"></td>
  </tr>
</table>

### Configuración del Sistema y Mantenimiento

Ajustes globales de la plataforma, asignación de recursos, facturación y políticas de seguridad.

<table align="center" width="100%">
  <tr>
    <td align="center" width="50%"><strong>Configuración General</strong><br><br><img src="image/Settings.png" width="350"></td>
    <td align="center" width="50%"><strong>Métricas de Almacenamiento</strong><br><br><img src="image/Storage.png" width="350"></td>
  </tr>
  <tr>
    <td align="center" width="50%"><strong>Facturación y Licenciamiento</strong><br><br><img src="image/Billing.png" width="350"></td>
    <td align="center" width="50%"><strong>Políticas de Seguridad</strong><br><br><img src="image/Security.png" width="350"></td>
  </tr>
</table>

### Notificaciones y Soporte Técnico

Módulos transversales para mantener al usuario informado e integrado con la documentación de las API.

<table align="center" width="100%">
  <tr>
    <td align="center" width="33%"><strong>Notificaciones</strong><br><br><img src="image/Notifications.png" width="250"></td>
    <td align="center" width="33%"><strong>Centro de Ayuda</strong><br><br><img src="image/Help.png" width="250"></td>
    <td align="center" width="33%"><strong>Documentación API (Swagger)</strong><br><br><img src="image/Swagger.png" width="250"></td>
  </tr>
</table>

---

## Arquitectura Técnica

El proyecto ha sido desarrollado siguiendo estrictamente los principios de **Clean Architecture**, asegurando un bajo acoplamiento y alta cohesión mediante la separación en 4 capas fundamentales:

| Capa               | Responsabilidad Técnica                                                                                                             |
| ------------------ | ----------------------------------------------------------------------------------------------------------------------------------- |
| **Domain**         | Contiene las entidades core del negocio, interfaces de dominio y excepciones de negocio puras. No posee dependencias externas.      |
| **Application**    | Implementa los casos de uso del sistema. Contiene la lógica orquestadora, servicios de aplicación, DTOs y validaciones.             |
| **Infrastructure** | Implementa el acceso a datos (Entity Framework Core), repositorios, servicios externos y manejo del sistema de archivos físico.     |
| **Web**            | Capa de presentación (Frontend en Angular) y controladores API REST (Backend). Maneja el ruteo HTTP y la inyección de dependencias. |

## Estándares y Patrones de Diseño

El código base respeta los principios **SOLID** y aplica estándares de la industria para garantizar su escalabilidad y mantenibilidad a largo plazo.

**Arquitectónicos:**

- Clean Architecture & Layered Architecture.

**De Diseño Estructural y Creacional:**

- Repository & Unit of Work (Abstracción de persistencia y transaccionalidad).
- Dependency Injection (Inversión de control).
- Service Layer (Encapsulamiento de lógica).
- Factory Method & Options Pattern (Configuraciones tipadas y creación segura de objetos).

**Implementaciones Técnicas:**

- **Data Transfer Objects (DTO):** Aislamiento del modelo de dominio.
- **Mapping (Mapster):** Transformación eficiente entre Entidades y DTOs.
- **Fluent Validation:** Validaciones de entrada de datos declarativas y separadas de la lógica de negocio.
- **Pipeline Middleware:** Procesamiento global de excepciones y solicitudes HTTP.

**Patrones Funcionales:**

- **Result Pattern:** Manejo predecible de respuestas y errores, evitando el uso excesivo de excepciones.
- **Maybe Monad & Railway-Oriented Programming:** Flujos de ejecución seguros contra valores nulos y encadenamiento de operaciones lógicas.

---

## Plan de Desarrollo (Roadmap)

El desarrollo del producto es iterativo. Las siguientes características conforman la hoja de ruta para las próximas versiones:

- [x] Implementación de políticas de bloqueo de cuenta por intentos de acceso fallidos.
- [ ] Integración de mecanismo Token Refresh para sesiones JWT ininterrumpidas.
- [ ] Autenticación federada mediante Google OAuth (SSO).
- [ ] Flujo seguro de recuperación y restablecimiento de contraseñas.
- [ ] Motor de búsqueda avanzada con filtros por metadatos (fecha, tamaño, tipo).
- [ ] Implementación de un modelo de control de acceso basado en roles (RBAC) granular.
- [ ] Generación de enlaces públicos con expiración para el uso compartido de activos externos.
