# SkillSnap

Portafolio full-stack y rastreador de proyectos construido con Blazor WebAssembly y ASP.NET Core 8.

## Tecnologías

- **Backend** - ASP.NET Core 8 Web API, Entity Framework Core, SQLite
- **Frontend** - Blazor WebAssembly SPA
- **Autenticación** - ASP.NET Core Identity + JWT Bearer (HMAC-SHA256)
- **Caché** - IMemoryCache con patrón cache-aside

## Características

- CRUD de proyectos y habilidades via API REST
- Autenticación JWT con control de acceso basado en roles (Admin / User)
- Caché en memoria con invalidación automática en escritura
- Estado de sesión persistente en Blazor via localStorage

## Ejecutar localmente

**API**
```bash
cd SkillSnap.Api
dotnet run
```

**Cliente**
```bash
cd SkillSnap.Client
dotnet run
```

La API corre en `https://localhost:7000` y el cliente en `https://localhost:5001`.

Al iniciar por primera vez, los roles (Admin, User) se crean automáticamente. Usa `POST /api/seed` para cargar datos de prueba y `POST /api/auth/register` para crear un usuario.

## Estructura del proyecto

```
skillsnap/
|- SkillSnap.Api/        -> ASP.NET Core Web API
|  |- Controllers/       -> Auth, Projects, Skills, Seed
|  |- Models/            -> ApplicationUser, PortfolioUser, Project, Skill
|  |- Data/              -> SkillSnapContext (IdentityDbContext)
|- SkillSnap.Client/     -> Blazor WebAssembly
   |- Pages/             -> Login, Register, ProjectList, SkillTags
   |- Services/          -> AuthService, ProjectService, SkillService, UserSessionService
```
